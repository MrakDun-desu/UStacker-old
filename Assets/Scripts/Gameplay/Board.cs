using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.CheeseGeneration;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Spins;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Appliers;
using NLua;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Blockstacker.Gameplay
{
    public class Board : MonoBehaviour, IBoard
    {
        [SerializeField] private GameSettingsSO _settings;
        [SerializeField] private Transform _helperTransform;
        [SerializeField] private GameManager _manager;
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private SpriteRenderer _backgroundRenderer;
        [SerializeField] private Camera _camera;
        [SerializeField] private WarningPiece _warningPiece;
        [SerializeField] private BlockBase _cheeseBlockPrefab;
        [SerializeField] private CheeseCollection _cheeseCollectionPrefab;

        [Tooltip("Zoom percentage change with one scroll unit")] [Range(0, 1)] [SerializeField]
        private float _boardZoomFactor = .05f;

        [Range(0.00001f, 1)] [SerializeField] private float _minimumBoardScale = 0.1f;

        private readonly List<ClearableBlock[]> Blocks = new();
        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots =>
            Blocks
                .Select(line => line.Select(block => block is not null).ToList().AsReadOnly())
                .ToList().AsReadOnly();

        private Vector3 _dragStartPosition;
        private Vector3 _dragStarttransformPosition;
        private uint _height;
        private Vector3 _offset;
        private uint _width;

        private uint _currentCombo;
        private uint _currentBackToBack;
        private bool _comboActive;
        private bool _backToBackActive;
        private float _warningPieceTreshhold;

        private ObjectPool<CheeseCollection> _cheeseCollectionPool;
        private ObjectPool<BlockBase> _cheeseBlockPool;

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
        
        public uint CheeseHeight { get; private set; }

        public uint LethalHeight { get; set; }
        private Vector3 Up => transform.up * transform.localScale.y;

        private Vector2 CurrentOffset => new(
            transform.position.x + Width * .5f * transform.localScale.x,
            transform.position.y + Height * .5f * transform.localScale.y
        );

        public event Action LinesCleared;
        
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

            if (_settings.Objective.CheeseGeneration == GameSettings.Enums.CheeseGeneration.None ||
                !_settings.Objective.UseCustomCheeseScript) return;

            _cheeseCollectionPool = new ObjectPool<CheeseCollection>(
                () => Instantiate(_cheeseCollectionPrefab),
                cc => cc.gameObject.SetActive(true),
                cc => cc.gameObject.SetActive(false),
                cc => Destroy(cc.gameObject),
                true,
                (int)_height,
                (int)_height * 2);
            
            _cheeseBlockPool = new ObjectPool<BlockBase>(
                () => Instantiate(_cheeseBlockPrefab),
                b => b.gameObject.SetActive(true),
                b => b.gameObject.SetActive(false),
                b => Destroy(b.gameObject),
                true,
                (int)_height,
                (int)_height * 2);
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

            var middleButton = Mouse.current.middleButton;
            var mouse = Mouse.current;
            if (middleButton.wasPressedThisFrame)
            {
                _dragStartPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
                _dragStarttransformPosition = transform.position;
            }
            else if (middleButton.isPressed)
            {
                var currentPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
                var positionDifference = currentPosition - _dragStartPosition;
                transform.position = _dragStarttransformPosition + positionDifference;
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

            var slots = Slots;
            for (var y = lineNumber; y < Blocks.Count; y++)
            for (var x = 0; x < Blocks[y].Length; x++)
            {
                if (!slots[y][x]) continue;
                
                Blocks[y][x].transform.position -= Up;
            }
        }

        private uint CheckAndClearLines()
        {
            uint linesCleared = 0;
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
            
            return linesCleared;
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

        public void Place(Block block)
        {
            var blockPos = WorldSpaceToBoardPosition(block.transform.position);
            if (!IsEmpty(blockPos)) return;
            while (Blocks.Count <= blockPos.y) Blocks.Add(new Block[Width]);

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

            var linesCleared = CheckAndClearLines();

            var linesWereCleared = linesCleared > 0;
            var wasAllClear = Blocks.Count == 0;

            var piecePlacedMsg = 
                new PiecePlacedMessage
            {
                LinesCleared = linesCleared, WasAllClear = wasAllClear, Time = placementTime,
                WasSpin = lastSpinResult.WasSpin, WasSpinMini = lastSpinResult.WasSpinMini,
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
                case TopoutCondition.LethalHeightStrict:
                    if (!isCompletelyBelowLethal)
                        _manager.LoseGame();
                    break;
                case TopoutCondition.LethalHeightLoose:
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
        }

        public void ResetB2bAndCombo()
        {
            _backToBackActive = false;
            _currentBackToBack = 0;
            _comboActive = false;
            _currentCombo = 0;
        }
        
        public void AddCheeseLines(IEnumerable<IEnumerable<bool>> slots)
        {
            
            foreach (var slot in slots)
            {
                if (slot.Count() != Width) continue;
                CheeseHeight++;
            }
        }

        public void AddCheeseLines(LuaTable slotsTable)
        {
            List<List<bool>> slots = new();
            foreach (var entry in slotsTable)
            {
                if (entry is not LuaTable line) continue;
                
                slots.Add(new List<bool>());
                foreach (var slotObject in line)
                {
                    if (slotObject is bool slot)
                        slots[-1].Add(slot);
                }

            }
            AddCheeseLines(slots);
        }

    }
}