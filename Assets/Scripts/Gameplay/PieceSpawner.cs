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
        private readonly List<PieceContainer> _previewContainers = new();
        private PiecePreviews _previews;
        private IRandomizer _randomizer { get; set; }

        public GameSettingsSO.SettingsContainer GameSettings { private get; set; }

        private void Awake()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
            _mediator.Register<GameStateChangedMessage>(InitializeSeed, 10);
            _mediator.Register<SeedSetMessage>(message => _randomizer?.Reset(message.Seed));
            
            FirstTimeInitialize();
        }

        private void OnDestroy()
        {
            foreach (var pool in _piecePools.Values)
                pool.Dispose();
        }

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            if (message.NewState == GameState.Initializing)
            {
                EmptyAllContainers();
                PrespawnPieces();
            }
            
            if (message is {PreviousState: GameState.Initializing or GameState.GameStartCountdown, NewState: GameState.Running})
                SpawnPiece();
        }

        private void InitializeSeed(GameStateChangedMessage message)
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
        
        private void FirstTimeInitialize()
        {
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
            
            _randomizer.Reset(GameSettings.General.ActiveSeed);

            SetAvailablePieces(_availablePieces);

            for (var i = 0; i < GameSettings.General.NextPieceCount; i++)
            {
                var pieceContainer = Instantiate(_pieceContainerPrefab, _board.transform);
                pieceContainer.transform.localPosition = new Vector3(
                    (int)_board.Width,
                    (int)_board.Height - PieceContainer.Height * (i + 1)
                );
                _previewContainers.Add(pieceContainer);
            }

            _previews = new PiecePreviews(_previewContainers);
        }

        public void PrespawnPieces()
        {
            foreach (var nextPieceType in _previewContainers.Select(_ => _randomizer.GetNextPiece()))
            {
                if (nextPieceType is null)
                    return;

                if (!_piecePools.ContainsKey(nextPieceType))
                {
                    _mediator.Send(new GameCrashedMessage($"Randomizer returned an invalid piece type {nextPieceType}"));
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
            newPiece.SetBoard(_board);
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

            var nextPiece = AppSettings.Sound.HearNextPieces ? _previews.GetFirstPieceType() : string.Empty;

            _mediator.Send(new PieceSpawnedMessage(piece.Type, nextPiece, spawnTime));

            if (!_board.CanPlace(piece))
                CantSpawn.Invoke(spawnTime);
        }

        public void EmptyAllContainers()
        {
            foreach (var piece in _previewContainers.Select(container => container.SwapPiece(null))
                         .Where(piece => piece != null)) piece.ReleaseFromPool();

            _containersEmpty = true;
        }

        private void SetAvailablePieces(PieceDictionary pieces)
        {
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
            newPiece.SourcePool = _piecePools[pieceType];
            return newPiece;
        }
    }
}