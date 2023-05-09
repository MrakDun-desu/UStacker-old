
/************************************
Board.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UStacker.Common.Extensions;
using UStacker.Gameplay.Blocks;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.GarbageGeneration;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Pieces;
using UStacker.Gameplay.Spins;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.Appliers;

namespace UStacker.Gameplay
{
    public class Board : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Transform _helperTransform;
        [SerializeField] private SpriteRenderer _backgroundRenderer;
        [SerializeField] private WarningPiece _warningPiece;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private ClearableBlock _garbageBlockPrefab;
        [SerializeField] private GarbageLayer _garbageLayerPrefab;
        [SerializeField] private RectTransform _statsCanvasTransform;

        [SerializeField] private UnityEvent<double> ToppedOut;

        private readonly List<ClearableBlock[]> Blocks = new();
        private bool _awake;

        private bool _backToBackActive;
        private bool _comboActive;
        private uint _currentBackToBack;
        private uint _currentCombo;

        private GameSettingsSO.SettingsContainer _gameSettings;

        private ObjectPool<ClearableBlock> _garbageBlockPool;
        private IGarbageGenerator _garbageGenerator;
        private ObjectPool<GarbageLayer> _garbageLayerPool;
        private GarbageLayer _lastGarbageLayer;

        private float _warningPieceTreshhold;

        public IEnumerable<Vector3> BlockPositions =>
            Blocks.SelectMany(tf => tf.Where(block => block is not null).Select(block => block.transform.position));

        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots =>
            Blocks
                .Select(line => line.Select(block => block is not null).ToList().AsReadOnly())
                .ToList().AsReadOnly();

        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint GarbageHeight { get; private set; }
        public uint LethalHeight { get; private set; }

        private void Awake()
        {
            if (_awake)
                return;

            _awake = true;
            ChangeVisibility(AppSettings.Gameplay.BoardVisibility);
            _warningPieceTreshhold = AppSettings.Gameplay.WarningPieceTreshhold;

            InitializeGarbagePools();

            BoardVisibilityApplier.VisibilityChanged += ChangeVisibility;
            WarningPieceTreshholdApplier.TreshholdChanged += ChangeWarningPieceTreshhold;
        }

        private void OnEnable()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChanged);
            _mediator.Register<SeedSetMessage>(OnSeedSet);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChanged);
            _mediator.Unregister<SeedSetMessage>(OnSeedSet);
        }

        private void OnDestroy()
        {
            _garbageLayerPool?.Dispose();
            _garbageBlockPool?.Dispose();
            if (_garbageGenerator is CustomGarbageGenerator gen)
                gen.Dispose();

            BoardVisibilityApplier.VisibilityChanged -= ChangeVisibility;
            WarningPieceTreshholdApplier.TreshholdChanged -= ChangeWarningPieceTreshhold;
        }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            private get => _gameSettings;
            set
            {
                _gameSettings = value;
                Awake();
                Initialize();
            }
        }

        public event Action LinesCleared;

        private void OnSeedSet(SeedSetMessage message)
        {
            _garbageGenerator?.ResetState(message.Seed);
        }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            if (message.NewState != GameState.Initializing)
                return;

            ClearAllBlocks();
            ResetB2bAndCombo();
            _garbageGenerator?.GenerateGarbage(GameSettings.Objective.GarbageHeight, new PiecePlacedMessage());
        }

        private void Initialize()
        {
            InitializeDimensions();
            InitializeGarbageGenerator();
        }

        private void InitializeDimensions()
        {
            var boardDimensions = GameSettings.BoardDimensions;
            _backgroundRenderer.transform.localScale = new Vector3(
                boardDimensions.BoardWidth,
                boardDimensions.BoardHeight,
                1
            );

            Width = boardDimensions.BoardWidth;
            Height = boardDimensions.BoardHeight;
            LethalHeight = boardDimensions.LethalHeight;
            _statsCanvasTransform.sizeDelta =
                new Vector2(boardDimensions.BoardWidth + 200f, boardDimensions.BoardHeight + 200f);
        }

        private void InitializeGarbageGenerator()
        {
            if (_garbageGenerator is CustomGarbageGenerator gen)
            {
                gen.Dispose();
                _garbageGenerator = null;
            }

            if (GameSettings.Objective.GarbageGenerationType == GarbageGenerationType.None)
                return;

            var readonlyBoard = new GarbageBoardInterface(this);

            if (GameSettings.Objective.GarbageGenerationType.HasFlag(GarbageGenerationType.CustomFlag))
                _garbageGenerator = new CustomGarbageGenerator(readonlyBoard,
                    GameSettings.Objective.CustomGarbageScript, _mediator);
            else
                _garbageGenerator = new DefaultGarbageGenerator(
                    readonlyBoard, GameSettings.Objective.GarbageGenerationType);
        }

        private void InitializeGarbagePools()
        {
            _garbageLayerPool = new ObjectPool<GarbageLayer>(
                CreateGarbageLayer,
                layer => layer.gameObject.SetActive(true),
                layer => layer.gameObject.SetActive(false),
                DestroyGarbageLayer);

            _garbageBlockPool = new ObjectPool<ClearableBlock>(
                CreateGarbageBlock,
                block => block.Visibility = 1,
                block => block.Visibility = 0,
                b => Destroy(b.gameObject));
        }

        private GarbageLayer CreateGarbageLayer()
        {
            var newGarbageLayer = Instantiate(_garbageLayerPrefab, transform);
            newGarbageLayer.transform.localPosition = Vector3.zero;
            newGarbageLayer.SourcePool = _garbageLayerPool;
            newGarbageLayer.BlockSourcePool = _garbageBlockPool;

            return newGarbageLayer;
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
            if (Blocks.Count + _warningPieceTreshhold >= LethalHeight)
                _warningPiece.MakeVisible();
            else
                _warningPiece.MakeInvisible();
        }

        private void ChangeVisibility(float newAlpha)
        {
            _backgroundRenderer.color = _backgroundRenderer.color.WithAlpha(newAlpha);
        }

        private void ClearLine(int lineNumber)
        {
            if (Blocks.Count <= lineNumber) return;

            var slots = Slots;
            for (var i = 0; i < Blocks[lineNumber].Length; i++)
            {
                if (!slots[lineNumber][i]) continue;

                Blocks[lineNumber][i].Clear();
            }

            Blocks.RemoveAt(lineNumber);
            if (GarbageHeight > lineNumber)
                GarbageHeight--;

            slots = Slots;
            for (var y = lineNumber; y < Blocks.Count; y++)
            for (var x = 0; x < Blocks[y].Length; x++)
            {
                if (!slots[y][x]) continue;

                var blockTransform = Blocks[y][x].transform;
                var selfTransform = transform;
                blockTransform.position -= selfTransform.up * selfTransform.lossyScale.y;
            }
        }

        private void CheckAndClearLines(out uint linesCleared, out uint garbageLinesCleared)
        {
            linesCleared = 0;
            var garbageHeightStart = GarbageHeight;
            var slots = Slots;
            for (var y = Blocks.Count - 1; y >= GameSettings.BoardDimensions.BlockCutHeight; y--)
            {
                for (var x = 0; x < Blocks[y].Length; x++)
                {
                    if (!slots[y][x]) continue;

                    Blocks[y][x].Clear();
                }

                Blocks.RemoveAt(y);
            }

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

            garbageLinesCleared = garbageHeightStart - GarbageHeight;
        }

        private void SendPlacementMessage(uint linesCleared, uint garbageLinesCleared, bool wasAllClear,
            double placementTime, SpinResult lastResult, string pieceType, int totalRotation, Vector2Int totalMovement,
            Vector2Int[] blockPositions)
        {
            var brokenBtb = false;
            var brokenCombo = false;
            if (linesCleared > 0)
            {
                if (lastResult.WasSpin || lastResult.WasSpinMini || linesCleared >= 4)
                {
                    if (_backToBackActive)
                        _currentBackToBack++;
                    else _backToBackActive = true;
                }
                else
                {
                    if (_currentBackToBack > 0)
                        brokenBtb = true;
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
                    brokenCombo = true;
                _currentCombo = 0;
                _comboActive = false;
            }

            var newMessage = new PiecePlacedMessage(linesCleared, garbageLinesCleared,
                _currentCombo, _currentBackToBack,
                pieceType, wasAllClear, lastResult.WasSpin,
                lastResult.WasSpinMini, lastResult.WasSpinRaw, lastResult.WasSpinMiniRaw,
                brokenCombo, brokenBtb, totalRotation, totalMovement, blockPositions, placementTime);
            _mediator.Send(newMessage);
            _garbageGenerator?.GenerateGarbage(GameSettings.Objective.GarbageHeight - GarbageHeight, newMessage);
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

        public bool Place(Piece piece, double placementTime, int totalRotation, Vector2Int totalMovement,
            SpinResult lastSpinResult)
        {
            if (!CanPlace(piece)) return false;
            lastSpinResult ??= new SpinResult();

            var placedPositions = piece.BlockPositions.Select(WorldSpaceToBoardPosition).ToArray();

            var isPartlyBelowLethal = false;
            var isCompletelyBelowLethal = true;

            foreach (var block in piece.Blocks)
            {
                Place(block);
                var blockPos = WorldSpaceToBoardPosition(block.transform.position);
                if (blockPos.y < LethalHeight) isPartlyBelowLethal = true;
                if (blockPos.y >= LethalHeight) isCompletelyBelowLethal = false;
            }

            CheckAndClearLines(out var linesCleared, out var garbageLinesCleared);

            var linesWereCleared = linesCleared > 0;
            var wasAllClear = Blocks.Count == 0;

            HandleWarningPiece();

            SendPlacementMessage(
                linesCleared,
                garbageLinesCleared,
                wasAllClear,
                placementTime,
                lastSpinResult,
                piece.Type,
                totalRotation,
                totalMovement,
                placedPositions);

            if (GameSettings.Gravity.AllowClutchClears && linesWereCleared) return true;

            switch (GameSettings.Gravity.TopoutCondition)
            {
                case TopoutCondition.PieceSpawn:
                    break;
                case TopoutCondition.OneBlockAboveLethal:
                    if (!isCompletelyBelowLethal)
                        ToppedOut.Invoke(placementTime);
                    break;
                case TopoutCondition.AllBlocksAboveLethal:
                    if (!isPartlyBelowLethal)
                        ToppedOut.Invoke(placementTime);
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

        private void ResetB2bAndCombo()
        {
            _backToBackActive = false;
            _currentBackToBack = 0;
            _comboActive = false;
            _currentCombo = 0;
        }

        public void AddGarbageLayer(IList<List<bool>> garbageLines, bool addToLast)
        {
            if (garbageLines.Count <= 0)
                return;

            var newGarbageLayer = addToLast && _lastGarbageLayer is not null
                ? _lastGarbageLayer
                : _garbageLayerPool.Get();

            var height = 0;

            for (; height < garbageLines.Count; height++)
            {
                var line = garbageLines[height];
                if (line.Count != Width)
                {
                    garbageLines.RemoveAt(height);
                    height--;
                    continue;
                }

                for (var x = 0; x < line.Count; x++)
                {
                    if (!line[x]) continue;

                    var garbageBlock = _garbageBlockPool.Get();
                    var garbageBlockTransform = garbageBlock.transform;
                    garbageBlockTransform.SetParent(newGarbageLayer.transform);
                    garbageBlockTransform.localPosition = new Vector3(x + .5f, height + .5f);
                    newGarbageLayer.AddBlock(garbageBlock);
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

                var blockTransform = Blocks[y][x].transform;
                var selfTransform = transform;
                blockTransform.position += selfTransform.up * selfTransform.lossyScale.y;
            }

            for (var i = 0; i < height; i++)
                Blocks.Insert(0, new ClearableBlock[Width]);

            foreach (var block in newGarbageLayer.Blocks) Place(block);

            newGarbageLayer.TriggerBlocksAdded();
        }

        // for future use
        [ContextMenu("Rotate by 30 deg")]
        private void RotateBy30()
        {
            var up = transform.lossyScale.y * transform.up;
            var right = transform.lossyScale.x * transform.right;

            transform.RotateAround(transform.position + Width * .5f * right + Height * .5f * up,
                Vector3.forward, 30f);
        }
    }
}
/************************************
end Board.cs
*************************************/
