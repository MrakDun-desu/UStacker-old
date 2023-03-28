using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Pieces;
using UStacker.Gameplay.Randomizers;
using UStacker.GameSettings;
using UStacker.GlobalSettings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.InputProcessing;
using UStacker.GameSettings.Enums;

namespace UStacker.Gameplay
{
    public class PieceSpawner : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Board _board;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private WarningPiece _warningPiece;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private PieceDictionary _availablePieces;
        [SerializeField] private PieceContainer _pieceContainerPrefab;
        [SerializeField] private UnityEvent<double> CantSpawn;

        private readonly Dictionary<string, ObjectPool<Piece>> _piecePools = new();
        private bool _containersEmpty = true;
        private int _defaultPoolCapacity;
        private readonly List<PieceContainer> _activePreviews = new();
        private ObjectPool<PieceContainer> _previewsPool;
        private PiecePreviews _previews;
        private IRandomizer _randomizer { get; set; }

        private GameSettingsSO.SettingsContainer _gameSettings;

        public GameSettingsSO.SettingsContainer GameSettings
        {
            private get => _gameSettings;
            set
            {
                _gameSettings = value;
                Initialize();
            }
        }

        private void Awake()
        {
            _previewsPool = new ObjectPool<PieceContainer>(
                () => Instantiate(_pieceContainerPrefab, _board.transform),
                preview => preview.gameObject.SetActive(true),
                preview => preview.gameObject.SetActive(false),
                preview => Destroy(preview.gameObject), true, 6, 10);
        }

        private void OnEnable()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
            _mediator.Register<GameStateChangedMessage>(OnGameStateChanged, 10);
            _mediator.Register<SeedSetMessage>(OnSeedSet);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChange);
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChanged);
            _mediator.Unregister<SeedSetMessage>(OnSeedSet);
        }

        private void OnSeedSet(SeedSetMessage message)
        {
            _randomizer?.Reset(message.Seed);
        }

        private void OnDestroy()
        {
            foreach (var pool in _piecePools.Values)
                pool.Dispose();

            if (_randomizer is CustomRandomizer rand)
                rand.Dispose();
        }

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            if (message.NewState == GameState.Initializing)
            {
                EmptyAllContainers();
                PrespawnPieces();
            }

            if (message is
                {
                    PreviousState: GameState.Initializing or GameState.StartCountdown, NewState: GameState.Running
                })
                SpawnPiece();
        }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            if (message.NewState != GameState.Initializing)
                return;

            if (message.IsReplay)
            {
                _mediator.Send(new SeedSetMessage(GameSettings.General.ActiveSeed));
                return;
            }

            if (GameSettings.General.UseCustomSeed)
                GameSettings.General.ActiveSeed = GameSettings.General.CustomSeed;
            else
            {
                var seed1 = (ulong) ((long) UnityEngine.Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                var seed2 = (ulong) ((long) UnityEngine.Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                GameSettings.General.ActiveSeed = seed1 + (seed2 << 32);
            }

            _mediator.Send(new SeedSetMessage(GameSettings.General.ActiveSeed));
        }

        private void Initialize()
        {
            if (_randomizer is CustomRandomizer rand)
                rand.Dispose();

            _randomizer = GameSettings.General.RandomizerType switch
            {
                RandomizerType.SevenBag => new CountPerBagRandomizer(_availablePieces.Keys),
                RandomizerType.FourteenBag => new CountPerBagRandomizer(_availablePieces.Keys, 2),
                RandomizerType.Stride => new StrideRandomizer(_availablePieces.Keys),
                RandomizerType.Random => new RandomRandomizer(_availablePieces.Keys),
                RandomizerType.Classic => new ClassicRandomizer(_availablePieces.Keys),
                RandomizerType.Pairs => new PairsRandomizer(_availablePieces.Keys),
                RandomizerType.Custom => new CustomRandomizer(
                    _availablePieces.Keys,
                    GameSettings.General.CustomRandomizerScript,
                    _mediator),
                _ => throw new IndexOutOfRangeException()
            };

            SetAvailablePieces(_availablePieces);

            while (GameSettings.General.NextPieceCount < _activePreviews.Count)
            {
                _previewsPool.Release(_activePreviews[0]);
                _activePreviews.RemoveAt(0);
            }

            while (_activePreviews.Count < GameSettings.General.NextPieceCount)
            {
                var pieceContainer = _previewsPool.Get();
                pieceContainer.transform.localPosition = new Vector3(
                    (int) _board.Width,
                    (int) _board.Height - PieceContainer.HEIGHT * (_activePreviews.Count + 1)
                );
                _activePreviews.Add(pieceContainer);
            }

            _previews = new PiecePreviews(_activePreviews);
        }

        public void PrespawnPieces()
        {
            foreach (var nextPieceType in _activePreviews.Select(_ => _randomizer.GetNextPiece()))
            {
                if (nextPieceType is null)
                    return;

                if (!_piecePools.ContainsKey(nextPieceType))
                {
                    _mediator.Send(
                        new GameCrashedMessage($"Randomizer returned an invalid piece type {nextPieceType}"));
                    return;
                }

                var nextPiece = _piecePools[nextPieceType].Get();
                nextPiece.SetBoard(_board);
                _previews.AddPiece(nextPiece);
            }

            _containersEmpty = false;
        }

        public void SpawnPiece()
        {
            SpawnPiece(0d);
        }

        public bool SpawnPiece(double spawnTime)
        {
            if (_containersEmpty) return false;

            var newPieceType = _randomizer.GetNextPiece();

            if (newPieceType is null)
                return false;

            if (!_piecePools.ContainsKey(newPieceType))
            {
                _mediator.Send(new GameCrashedMessage($"Randomizer returned an invalid piece type {newPieceType}"));
                return false;
            }

            var newPiece = _piecePools[newPieceType].Get();
            var nextPiece = _previews.AddPiece(newPiece);

            SpawnPiece(nextPiece, spawnTime);
            return true;
        }

        public void SpawnPiece(Piece piece, double spawnTime)
        {
            var boardTransform = _board.transform;
            var piecePos = new Vector3(
                (int) (GameSettings.BoardDimensions.BoardWidth / 2u),
                (int) GameSettings.BoardDimensions.PieceSpawnHeight,
                boardTransform.position.z
            );

            var pieceTransform = piece.transform;
            pieceTransform.SetParent(boardTransform);
            pieceTransform.localScale = Vector3.one;
            pieceTransform.localPosition = piecePos + new Vector3(piece.SpawnOffset.x, piece.SpawnOffset.y);

            _inputProcessor.ActivePiece = piece;

            var rotationSystem = GameSettings.Controls.ActiveRotationSystem;
            var rotation = rotationSystem.GetKickTable(piece.Type).StartState;
            piece.RotationState = rotation;
            piece.Rotate((int) rotation);

            _inputProcessor.HandlePreSpawnBufferedInputs(spawnTime, out var cancelSpawn);

            if (cancelSpawn) return;
            
            _warningPiece.SetPiece(_previews.GetFirstPiece());

            var nextPiece = AppSettings.Sound.HearNextPieces ? _previews.GetFirstPiece()?.Type : string.Empty;

            _mediator.Send(new PieceSpawnedMessage(piece.Type, nextPiece, spawnTime));

            if (_board.CanPlace(piece)) return;
            if (GameSettings.Controls.AllowAutomaticPreSpawnRotation)
            {
                if (!_inputProcessor.TryAutomaticPreSpawnRotation(spawnTime))
                    CantSpawn.Invoke(spawnTime);
            }
            else
                CantSpawn.Invoke(spawnTime);
        }

        public void EmptyAllContainers()
        {
            foreach (var piece in _activePreviews.Select(container => container.SwapPiece(null))
                         .Where(piece => piece != null)) piece.ReleaseFromPool();

            _containersEmpty = true;
        }

        private void SetAvailablePieces(PieceDictionary pieces)
        {
            foreach (var pool in _piecePools.Values)
                pool.Dispose();

            _piecePools.Clear();

            var blockCount = GameSettings.BoardDimensions.BoardHeight * GameSettings.BoardDimensions.BoardWidth;
            _defaultPoolCapacity = (int) (blockCount / 25u);
            var maxSize = (int) (blockCount / 3u);

            foreach (var piecePair in pieces)
            {
                _piecePools.Add(piecePair.Key, new ObjectPool<Piece>(
                    () => CreatePiece(piecePair.Value, piecePair.Key),
                    p => p.Activate(),
                    p => p.Deactivate(),
                    p => Destroy(p.gameObject),
                    true,
                    _defaultPoolCapacity,
                    maxSize
                ));
            }
        }

        private Piece CreatePiece(Piece piece, string pieceType)
        {
            var newPiece = Instantiate(piece);
            newPiece.SetBoard(_board);
            newPiece.SourcePool = _piecePools[pieceType];
            return newPiece;
        }
    }
}