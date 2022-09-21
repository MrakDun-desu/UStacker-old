using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.GarbageGeneration;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Spins;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Appliers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Blockstacker.Gameplay
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _settings;
        [SerializeField] private Transform _helperTransform;
        [SerializeField] private GameManager _manager;
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private SpriteRenderer _backgroundRenderer;
        [SerializeField] private Camera _camera;
        [SerializeField] private WarningPiece _warningPiece;
        [SerializeField] private ClearableBlock _garbageBlockPrefab;
        [SerializeField] private GarbageLayer _garbageLayerPrefab;

        [Tooltip("Zoom percentage change with one scroll unit")] [Range(0, 1)] [SerializeField]
        private float _boardZoomFactor = .05f;

        [Range(0.00001f, 1)] [SerializeField] private float _minimumBoardScale = 0.1f;

        private readonly List<ClearableBlock[]> Blocks = new();

        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots =>
            Blocks
                .Select(line => line.Select(block => block is not null).ToList().AsReadOnly())
                .ToList().AsReadOnly();

        private Vector3 _dragStartPosition;
        private Vector3 _dragStartTransformPosition;
        private uint _height;
        private Vector3 _offset;
        private uint _width;

        private uint _currentCombo;
        private uint _currentBackToBack;
        private bool _comboActive;
        private bool _backToBackActive;
        private float _warningPieceTreshhold;

        private ObjectPool<GarbageLayer> _garbageLayerPool;
        private ObjectPool<ClearableBlock> _garbageBlockPool;

        private GarbageLayer _lastGarbageLayer;

        public uint Width
        {
            get => _width;
            set
            {
                _width = value;
                var mytransform = transform;
                var myPos = mytransform.position;
                mytransform.position =
                    new Vector3(-value * .5f * mytransform.localScale.x + _offset.x, myPos.y, myPos.z);
            }
        }

        public uint Height
        {
            get => _height;
            set
            {
                _height = value;
                var mytransform = transform;
                var myPos = mytransform.position;
                mytransform.position =
                    new Vector3(myPos.x, -value * .5f * mytransform.localScale.y + _offset.y, myPos.z);
            }
        }

        public uint GarbageHeight { get; private set; }
        public uint LethalHeight { get; set; }
        private Vector3 Up => transform.up * transform.localScale.y;

        private Vector2 CurrentOffset => new(
            transform.position.x + Width * .5f * transform.localScale.x,
            transform.position.y + Height * .5f * transform.localScale.y
        );

        public event Action LinesCleared;

        public IGarbageGenerator GarbageGenerator;

        private void Start()
        {
            _offset = AppSettings.Gameplay.BoardOffset;
            transform.position += _offset;

            ChangeBoardZoom(AppSettings.Gameplay.BoardZoom);
            ChangeVisibility(AppSettings.Gameplay.BoardVisibility);
            _warningPieceTreshhold = AppSettings.Gameplay.WarningPieceTreshhold;

            BoardVisibilityApplier.VisibilityChanged += ChangeVisibility;
            BoardZoomApplier.BoardZoomChanged += ChangeBoardZoom;
            WarningPieceTreshholdApplier.TreshholdChanged += ChangeWarningPieceTreshhold;
            
            _mediator.Register<GameStartedMessage>(OnGameStarted);
        }
        
        private void Update()
        {
            HandleBoardZooming();
            HandleBoardDrag();
        }

        private void OnDestroy()
        {
            BackgroundVisibilityApplier.VisibilityChanged -= ChangeVisibility;
            BoardZoomApplier.BoardZoomChanged -= ChangeBoardZoom;
            WarningPieceTreshholdApplier.TreshholdChanged -= ChangeWarningPieceTreshhold;
            
            _mediator.Unregister<GameStartedMessage>(OnGameStarted);
        }

        private void OnGameStarted(GameStartedMessage message)
        {
            GarbageGenerator?.ResetState(message.Seed);
            GarbageGenerator?.GenerateGarbage(_settings.Objective.GarbageHeight);
        }

        private GarbageLayer CreateGarbageLayer()
        {
            var newCheeseCollection = Instantiate(_garbageLayerPrefab, transform);
            newCheeseCollection.transform.localPosition = Vector3.zero;
            newCheeseCollection.SourcePool = _garbageLayerPool;
            newCheeseCollection.BlockSourcePool = _garbageBlockPool;

            return newCheeseCollection;
        }

        private void DestroyGarbageLayer(GarbageLayer layer)
        {
            foreach (var block in layer.Blocks)
            {
                _garbageBlockPool.Release(block);
                block.transform.SetParent(null);
            }

            Destroy(layer.gameObject);
        }

        private ClearableBlock CreateGarbageBlock()
        {
            var newGarbageBlock = Instantiate(_garbageBlockPrefab);
            newGarbageBlock.Board = this;
            return newGarbageBlock;
        }

        private void ChangeWarningPieceTreshhold(float newTreshhold)
        {
            _warningPieceTreshhold = newTreshhold;
            HandleWarningPiece();
        }

        private void HandleWarningPiece()
        {
            var blockCount = Blocks.Count;
            var lethalHeight = LethalHeight;
            if (blockCount + _warningPieceTreshhold >= lethalHeight)
                _warningPiece.MakeVisible();
            else
                _warningPiece.MakeInvisible();
        }

        private void ChangeVisibility(float newAlpha)
        {
            _backgroundRenderer.color = _backgroundRenderer.color.WithAlpha(newAlpha);
        }

        private void HandleBoardZooming()
        {
            const float ONE_SCROLL_UNIT = 1 / 120f;

            if (!AppSettings.Gameplay.CtrlScrollToChangeBoardZoom) return;

            if (!Keyboard.current.ctrlKey.isPressed) return;

            var mouseScroll = Mouse.current.scroll.ReadValue().y * ONE_SCROLL_UNIT;
            var newScale = transform.localScale.x + mouseScroll * _boardZoomFactor;
            var newZoom = newScale < _minimumBoardScale ? _minimumBoardScale : newScale;
            ChangeBoardZoom(newZoom);
            AppSettings.Gameplay.BoardZoom = newZoom;
            AppSettings.Gameplay.BoardOffset = CurrentOffset;
        }

        private void HandleBoardDrag()
        {
            if (!AppSettings.Gameplay.DragMiddleButtonToRepositionBoard) return;

            var mouse = Mouse.current;
            var middleButton = mouse.middleButton;
            if (middleButton.wasPressedThisFrame)
            {
                _dragStartPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
                _dragStartTransformPosition = transform.position;
            }
            else if (middleButton.isPressed)
            {
                var currentPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
                var positionDifference = currentPosition - _dragStartPosition;
                transform.position = _dragStartTransformPosition + positionDifference;
                AppSettings.Gameplay.BoardOffset = CurrentOffset;
                _offset = CurrentOffset;
            }
        }

        private void ChangeBoardZoom(float zoom)
        {
            if (Mathf.Abs(zoom - transform.localScale.x) < .01f) return;
            var mytransform = transform;
            mytransform.localScale = new Vector3(zoom, zoom, 1);
            mytransform.position = new Vector3(-zoom * .5f * Width, -zoom * .5f * Height, 1);
        }

        private void ClearLine(int lineNumber)
        {
            if (Blocks.Count <= lineNumber) return;
            for (var i = 0; i < Blocks[lineNumber].Length; i++)
            {
                if (!Slots[lineNumber][i]) continue;

                Blocks[lineNumber][i].Clear();
            }

            Blocks.RemoveAt(lineNumber);
            if (GarbageHeight > lineNumber)
                GarbageHeight--;

            var slots = Slots;
            for (var y = lineNumber; y < Blocks.Count; y++)
            for (var x = 0; x < Blocks[y].Length; x++)
            {
                if (!slots[y][x]) continue;

                Blocks[y][x].transform.position -= Up;
            }
        }

        private void CheckAndClearLines(out uint linesCleared, out uint cheeseLinesCleared)
        {
            linesCleared = 0;
            var cheeseHeightStart = GarbageHeight;
            for (var i = 0; i < Blocks.Count; i++)
            {
                var isFull = Slots[i].All(blockExists => blockExists);
                if (!isFull) continue;
                linesCleared++;
                ClearLine(i);
                i--;
            }

            if (linesCleared > 0)
                LinesCleared?.Invoke();

            cheeseLinesCleared = cheeseHeightStart - GarbageHeight;
        }

        private void SendPlacementMessage(PiecePlacedMessage midgameMessage)
        {
            if (midgameMessage.LinesCleared > 0)
            {
                if (midgameMessage.WasSpin || midgameMessage.WasSpinMini || midgameMessage.LinesCleared >= 4)
                {
                    if (_backToBackActive)
                        _currentBackToBack++;
                    else _backToBackActive = true;
                }
                else
                {
                    if (_currentBackToBack > 0)
                        midgameMessage.BrokenBackToBack = true;
                    _backToBackActive = false;
                    _currentBackToBack = 0;
                }

                if (_comboActive)
                    _currentCombo++;
                else _comboActive = true;
            }
            else
            {
                if (_currentCombo > 0)
                    midgameMessage.BrokenCombo = true;
                _currentCombo = 0;
                _comboActive = false;
            }

            midgameMessage.CurrentCombo = _currentCombo;
            midgameMessage.CurrentBackToBack = _currentBackToBack;

            _mediator.Send(midgameMessage);
            GarbageGenerator?.GenerateGarbage(_settings.Objective.GarbageHeight - GarbageHeight, midgameMessage);
        }

        public Vector2Int WorldSpaceToBoardPosition(Vector3 worldSpacePos)
        {
            _helperTransform.position = worldSpacePos;
            var localPosition = _helperTransform.localPosition;
            return new Vector2Int(Mathf.FloorToInt(localPosition.x),
                Mathf.FloorToInt(localPosition.y));
        }

        private bool IsEmpty(Vector2Int blockPosition)
        {
            if (blockPosition.x < 0 || blockPosition.x >= Width ||
                blockPosition.y < 0) return false;

            if (Blocks.Count <= blockPosition.y) return true;

            return Blocks[blockPosition.y][blockPosition.x] is null;
        }

        public bool IsEmpty(Vector3 realPosition, Vector2Int offset = new())
        {
            var boardPosition = WorldSpaceToBoardPosition(realPosition);
            return IsEmpty(boardPosition + offset);
        }

        public bool CanPlace(Piece piece, Vector2Int offset = new())
        {
            return piece.Blocks.All(block => IsEmpty(block.transform.position, offset));
        }

        public bool CanPlace(IEnumerable<BlockBase> blocks, Vector2Int offset = new())
        {
            return blocks.All(block => IsEmpty(block.transform.position, offset));
        }

        private void Place(ClearableBlock block)
        {
            var blockPos = WorldSpaceToBoardPosition(block.transform.position);
            if (!IsEmpty(blockPos)) return;
            while (Blocks.Count <= blockPos.y) Blocks.Add(new ClearableBlock[Width]);

            Blocks[blockPos.y][blockPos.x] = block;
        }

        public bool Place(Piece piece, double placementTime, SpinResult lastSpinResult = null)
        {
            if (!CanPlace(piece)) return false;
            lastSpinResult ??= new SpinResult();

            var isPartlyBelowLethal = false;
            var isCompletelyBelowLethal = true;

            foreach (var block in piece.Blocks)
            {
                Place(block);
                var blockPos = WorldSpaceToBoardPosition(block.transform.position);
                if (blockPos.y < LethalHeight) isPartlyBelowLethal = true;
                if (blockPos.y >= LethalHeight) isCompletelyBelowLethal = false;
            }

            CheckAndClearLines(out var linesCleared, out var cheeseLinesCleared);

            var linesWereCleared = linesCleared > 0;
            var wasAllClear = Blocks.Count == 0;

            var piecePlacedMsg =
                new PiecePlacedMessage
                {
                    LinesCleared = linesCleared, GarbageLinesCleared = cheeseLinesCleared, WasAllClear = wasAllClear,
                    Time = placementTime, WasSpin = lastSpinResult.WasSpin, WasSpinMini = lastSpinResult.WasSpinMini,
                    WasSpinRaw = lastSpinResult.WasSpinRaw, WasSpinMiniRaw = lastSpinResult.WasSpinMiniRaw,
                    PieceType = piece.Type
                };

            HandleWarningPiece();

            SendPlacementMessage(piecePlacedMsg);

            if (_settings.Rules.BoardDimensions.AllowClutchClears && linesWereCleared) return true;

            switch (_settings.Rules.BoardDimensions.TopoutCondition)
            {
                case TopoutCondition.PieceSpawn:
                    break;
                case TopoutCondition.OneBlockAboveLethal:
                    if (!isCompletelyBelowLethal)
                        _manager.LoseGame();
                    break;
                case TopoutCondition.AllBlocksAboveLethal:
                    if (!isPartlyBelowLethal)
                        _manager.LoseGame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return linesWereCleared;
        }

        public void ClearAllBlocks()
        {
            foreach (var block in Blocks.SelectMany(line => line))
            {
                if (block == null) continue;
                block.Clear();
            }

            Blocks.Clear();
            GarbageHeight = 0;
            _lastGarbageLayer = null;
        }

        public void ResetB2bAndCombo()
        {
            _backToBackActive = false;
            _currentBackToBack = 0;
            _comboActive = false;
            _currentCombo = 0;
        }

        public void AddGarbageLayer(IList<List<bool>> slots, bool addToLast)
        {
            var newGarbageLayer = addToLast && _lastGarbageLayer is not null 
                ? _lastGarbageLayer 
                : _garbageLayerPool.Get();

            var height = 0;

            for (; height < slots.Count; height++)
            {
                var line = slots[height];
                if (line.Count != Width)
                {
                    slots.RemoveAt(height);
                    height--;
                    continue;
                }

                for (var x = 0; x < line.Count; x++)
                {
                    if (!line[x]) continue;

                    var cheeseBlock = _garbageBlockPool.Get();
                    var cheeseBlockTransform = cheeseBlock.transform;
                    cheeseBlockTransform.SetParent(newGarbageLayer.transform);
                    cheeseBlockTransform.localPosition = new Vector3(x + .5f, height + .5f);
                    cheeseBlockTransform.localScale = Vector3.one;
                    newGarbageLayer.AddBlock(cheeseBlock);
                }
            }

            if (height <= 0)
                return;

            _lastGarbageLayer = newGarbageLayer;
            GarbageHeight += (uint) height;

            var activeSlots = Slots;
            for (var y = 0; y < Blocks.Count; y++)
            for (var x = 0; x < Blocks[y].Length; x++)
            {
                if (!activeSlots[y][x]) continue;

                Blocks[y][x].transform.position += Up * height;
            }

            for (var i = 0; i < height; i++)
                Blocks.Insert(0, new ClearableBlock[Width]);

            foreach (var block in newGarbageLayer.Blocks)
            {
                Place(block);
            }
            
            newGarbageLayer.TriggerBlocksAdded();
        }

        public void InitializeGarbagePools()
        {
            _garbageLayerPool = new ObjectPool<GarbageLayer>(
                CreateGarbageLayer,
                cc => cc.gameObject.SetActive(true),
                cc => cc.gameObject.SetActive(false),
                DestroyGarbageLayer,
                true,
                (int) (_height / 2u),
                (int) _height);

            _garbageBlockPool = new ObjectPool<ClearableBlock>(
                CreateGarbageBlock,
                b => b.gameObject.SetActive(true),
                null,
                b => Destroy(b.gameObject),
                true,
                (int) (_height * _width / 2u),
                (int) (_height * _width));
        }

    }
}