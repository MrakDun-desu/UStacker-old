using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Spins;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Appliers;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [Tooltip("Zoom percentage change with one scroll unit")] [Range(0, 1)] [SerializeField]
        private float _boardZoomFactor = .05f;

        [Range(0.00001f, 1)] [SerializeField] private float _minimumBoardScale = 0.1f;

        private readonly List<Block[]> Blocks = new();

        private Vector3 _dragStartPosition;
        private Vector3 _dragStartTransformPosition;
        private uint _height;
        private Vector3 _offset;
        private uint _width;

        private uint _currentCombo;
        private uint _currentBackToBack;
        private bool _comboActive;
        private bool _backToBackActive;

        public uint Width
        {
            get => _width;
            set
            {
                _width = value;
                var myTransform = transform;
                var myPos = myTransform.position;
                myTransform.position =
                    new Vector3(-value * .5f * myTransform.localScale.x + _offset.x, myPos.y, myPos.z);
            }
        }

        public uint Height
        {
            get => _height;
            set
            {
                _height = value;
                var myTransform = transform;
                var myPos = myTransform.position;
                myTransform.position =
                    new Vector3(myPos.x, -value * .5f * myTransform.localScale.y + _offset.y, myPos.z);
            }
        }

        public uint LethalHeight { get; set; }
        private Vector3 Up => transform.up * transform.localScale.y;
        private Vector3 Right => transform.right * transform.localScale.x;

        private Vector2 CurrentOffset => new(
            transform.position.x + Width * .5f * transform.localScale.x,
            transform.position.y + Height * .5f * transform.localScale.y
        );


        private void Start()
        {
            _offset = AppSettings.Gameplay.BoardOffset;
            transform.position += _offset;

            ChangeBoardZoom(AppSettings.Gameplay.BoardZoom);
            ChangeVisibility(AppSettings.Gameplay.BoardVisibility);

            BoardVisibilityApplier.VisibilityChanged += ChangeVisibility;
            BoardZoomApplier.BoardZoomChanged += ChangeBoardZoom;
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
            var myTransform = transform;
            myTransform.localScale = new Vector3(zoom, zoom, 1);
            myTransform.position = new Vector3(-zoom * .5f * Width, -zoom * .5f * Height, 1);
        }

        private void ClearLine(int lineNumber)
        {
            if (Blocks.Count <= lineNumber) return;
            foreach (var block in Blocks[lineNumber])
            {
                if (block == null) continue;
                block.Clear();
            }

            Blocks.RemoveAt(lineNumber);
            for (var i = lineNumber; i < Blocks.Count; i++)
                foreach (var block in Blocks[i])
                {
                    if (block == null) continue;
                    block.transform.position -= Up;
                }
        }

        private uint CheckAndClearLines()
        {
            uint linesCleared = 0;
            for (var i = 0; i < Blocks.Count; i++)
            {
                var line = Blocks[i];
                var isFull = line.All(block => block != null);
                if (!isFull) continue;
                linesCleared++;
                ClearLine(i);
                i--;
            }

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
                    _backToBackActive = false;
                    _currentBackToBack = 0;
                }
                
                if (_comboActive)
                    _currentCombo++;
                else _comboActive = true;
            }
            else
            {
                _currentCombo = 0;
                _comboActive = false;
            }

            midgameMessage.CurrentCombo = _currentCombo;
            midgameMessage.CurrentBackToBack = _currentBackToBack;

            _mediator.Send(midgameMessage);
        }

        private Vector2Int WorldSpaceToBoardPosition(Vector3 worldSpacePos)
        {
            _helperTransform.position = worldSpacePos;
            var localPosition = _helperTransform.localPosition;
            return new Vector2Int(Mathf.FloorToInt(localPosition.x),
                Mathf.FloorToInt(localPosition.y));
        }

        public Vector3 BoardPositionToWorldSpace(Vector2Int boardPos)
        {
            return transform.position + boardPos.x * Right + boardPos.y * Up;
        }

        public bool IsEmpty(Vector2Int blockPosition)
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

        public bool CanPlace(IEnumerable<Transform> transforms, Vector2Int offset = new())
        {
            return transforms.All(tf => IsEmpty(tf.position, offset));
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
                WasSpin = lastSpinResult.WasSpin, WasSpinMini = lastSpinResult.WasSpinMini, PieceType = piece.PieceType
            };

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
    }
}