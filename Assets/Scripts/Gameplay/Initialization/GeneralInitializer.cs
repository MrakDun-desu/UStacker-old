using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UStacker.Gameplay.InputProcessing;
using UStacker.Gameplay.Randomizers;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace UStacker.Gameplay.Initialization
{
    public class GeneralInitializer : InitializerBase
    {
        private readonly GameReplay _replay;
        private readonly PieceDictionary _availablePieces;
        private readonly Board _board;
        private readonly InputProcessor _inputProcessor;
        private readonly bool _isRestarting;
        private readonly PieceContainer _pieceContainerPrefab;
        private readonly PieceSpawner _spawner;
        private readonly GameStateManager _stateManager;

        public GeneralInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO.SettingsContainer gameSettings,
            PieceDictionary availablePieces,
            PieceSpawner spawner,
            Board board,
            PieceContainer pieceContainerPrefab,
            InputProcessor inputProcessor,
            GameStateManager stateManager,
            GameReplay replay,
            bool isRestarting) : base(problemBuilder, gameSettings)
        {
            _availablePieces = availablePieces;
            _spawner = spawner;
            _board = board;
            _pieceContainerPrefab = pieceContainerPrefab;
            _inputProcessor = inputProcessor;
            _stateManager = stateManager;
            _replay = replay;
            _isRestarting = isRestarting;
        }

        public override void Execute()
        {
            var isReplay = _replay is not null;
            _stateManager.IsReplaying = isReplay;
            if (isReplay)
            {
                _inputProcessor.PlacementsList = _replay.PiecePlacementList;
                _inputProcessor.ActionList = _replay.ActionList;
            }
            else
            {
                _inputProcessor.ActionList = null;
                InitializeSeed();
            }

            InitializeRandomizer();
            if (_isRestarting) return;
            InitializePieceContainers();
            InitializePieceHolder();
        }

        private void InitializeSeed()
        {
            if (_gameSettings.General.UseCustomSeed)
                _gameSettings.General.ActiveSeed = _gameSettings.General.CustomSeed;
            else
            {
                var seed1 = (ulong) ((long) Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                var seed2 = (ulong) ((long) Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                _gameSettings.General.ActiveSeed = seed1 + (seed2 << 32);
            }
        }

        private void InitializeRandomizer()
        {
            if (_isRestarting)
            {
                _spawner.Randomizer.Reset(_gameSettings.General.ActiveSeed);
                return;
            }

            string validationErrors = null;

            IRandomizer randomizer = _gameSettings.General.RandomizerType switch
            {
                RandomizerType.SevenBag => new CountPerBagRandomizer(_availablePieces.Keys),
                RandomizerType.FourteenBag => new CountPerBagRandomizer(_availablePieces.Keys, 2),
                RandomizerType.Stride => new StrideRandomizer(_availablePieces.Keys),
                RandomizerType.Random => new RandomRandomizer(_availablePieces.Keys),
                RandomizerType.Classic => new ClassicRandomizer(_availablePieces.Keys),
                RandomizerType.Pairs => new PairsRandomizer(_availablePieces.Keys),
                RandomizerType.Custom => new CustomRandomizer(
                    _availablePieces.Keys,
                    _gameSettings.General.CustomRandomizerScript,
                    out validationErrors),
                _ => throw new IndexOutOfRangeException()
            };

            if (validationErrors is not null)
            {
                _errorBuilder.AppendLine(validationErrors);
                return;
            }

            _spawner.Randomizer = randomizer;
            _spawner.SetAvailablePieces(_availablePieces);
            _spawner.Randomizer.Reset(_gameSettings.General.ActiveSeed);
        }

        private void InitializePieceContainers()
        {
            var previewContainers = new List<PieceContainer>();
            for (var i = 0; i < _gameSettings.General.NextPieceCount; i++)
            {
                var pieceContainer = Object.Instantiate(_pieceContainerPrefab, _board.transform);
                pieceContainer.transform.localPosition = new Vector3(
                    (int)_board.Width,
                    (int)_board.Height - PieceContainer.Height * (i + 1)
                );
                previewContainers.Add(pieceContainer);
            }
            _spawner.SetPreviewContainers(previewContainers);
        }

        private void InitializePieceHolder()
        {
            if (!_gameSettings.Controls.AllowHold) return;
            var pieceHolder = Object.Instantiate(_pieceContainerPrefab, _board.transform);
            pieceHolder.transform.localPosition = new Vector3(
                -PieceContainer.Width,
                (int)_board.Height - PieceContainer.Height
            );

            _inputProcessor.PieceHolder = pieceHolder;

        }
    }
}