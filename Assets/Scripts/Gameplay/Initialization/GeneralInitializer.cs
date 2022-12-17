using System;
using System.Collections.Generic;
using System.Text;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Blockstacker.Gameplay.Initialization
{
    public class GeneralInitializer : InitializerBase
    {
        private readonly List<InputActionMessage> _actionList;
        private readonly PieceDictionary _availablePieces;
        private readonly Board _board;
        private readonly InputProcessor _inputProcessor;
        private readonly bool _isReplay;
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
            List<InputActionMessage> actionList,
            bool isRestarting,
            bool isReplay) : base(problemBuilder, gameSettings)
        {
            _availablePieces = availablePieces;
            _spawner = spawner;
            _board = board;
            _pieceContainerPrefab = pieceContainerPrefab;
            _inputProcessor = inputProcessor;
            _stateManager = stateManager;
            _actionList = actionList;
            _isRestarting = isRestarting;
            _isReplay = isReplay;
        }

        public override void Execute()
        {
            _stateManager.IsReplaying = _isReplay;
            if (_isReplay)
                _inputProcessor.ActionList = _actionList;
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
            _gameSettings.General.ActiveSeed = _gameSettings.General.UseCustomSeed
                ? _gameSettings.General.CustomSeed
                : Random.Range(int.MinValue, int.MaxValue);
        }

        private void InitializeRandomizer()
        {
            if (_isRestarting)
            {
                // if (_spawner is null)
                // {
                //     Debug.LogError("Spawner is null");
                //     return;
                // }
                _spawner.Randomizer.Reset(_gameSettings.General.ActiveSeed);
                return;
            }

            string validationErrors = null;

            var seed = _gameSettings.General.ActiveSeed;

            IRandomizer randomizer = _gameSettings.General.RandomizerType switch
            {
                RandomizerType.SevenBag => new CountPerBagRandomizer(_availablePieces.Keys, seed),
                RandomizerType.FourteenBag => new CountPerBagRandomizer(_availablePieces.Keys, seed, 2),
                RandomizerType.Stride => new StrideRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Random => new RandomRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Classic => new ClassicRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Pairs => new PairsRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Custom => new CustomRandomizer(
                    _availablePieces.Keys,
                    _gameSettings.General.CustomRandomizerScript,
                    seed,
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
        }

        private void InitializePieceContainers()
        {
            var previewContainers = new List<PieceContainer>();
            for (var i = 0; i < _gameSettings.General.NextPieceCount; i++)
            {
                var pieceContainer = Object.Instantiate(_pieceContainerPrefab, _board.transform);
                pieceContainer.transform.localPosition = new Vector3(
                    (int) _board.Width,
                    (int) _board.Height - PieceContainer.Height * (i + 1)
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
                (int) _board.Height - PieceContainer.Height
            );

            _inputProcessor.PieceHolder = pieceHolder;

        }
    }
}