using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Blockstacker.Common;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesGeneralInitializer : InitializerBase
    {
        private readonly PieceDictionary _availablePieces;
        private readonly Board _board;
        private readonly InputProcessor _inputProcessor;
        private readonly bool _isRestarting;
        private readonly PieceContainer _pieceContainerPrefab;
        private readonly PieceSpawner _spawner;

        public RulesGeneralInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings,
            PieceDictionary availablePieces,
            PieceSpawner spawner,
            Board board,
            PieceContainer pieceContainerPrefab,
            InputProcessor inputProcessor,
            bool isRestarting = false) : base(problemBuilder, gameSettings)
        {
            _availablePieces = availablePieces;
            _spawner = spawner;
            _board = board;
            _pieceContainerPrefab = pieceContainerPrefab;
            _inputProcessor = inputProcessor;
            _isRestarting = isRestarting;
        }

        public override void Execute()
        {
            InitializeSeed();
            InitializeRandomizer();
            
            if (_isRestarting) return;
            InitializePieceContainers();
            InitializePieceHolder();
        }

        private void InitializeSeed()
        {
             _gameSettings.Rules.General.ActiveSeed = _gameSettings.Rules.General.UseRandomSeed
                ? Random.Range(int.MinValue, int.MaxValue)
                : _gameSettings.Rules.General.SpecificSeed;
        }

        private void InitializeRandomizer()
        {
            if (_gameSettings.Rules.General.RandomizerType == RandomizerType.Custom)
            {
                var randomizerScriptPath =
                    Path.Combine(CustomizationPaths.Randomizers, _gameSettings.Rules.General.CustomRandomizerName);
                if (!File.Exists(randomizerScriptPath))
                {
                    _errorBuilder.AppendLine("Custom randomizer script not found.");
                    return;
                }

                _gameSettings.Rules.General.CustomRandomizerScript = File.ReadAllText(randomizerScriptPath);
            }

            string validationErrors = null;
            var seed = _gameSettings.Rules.General.ActiveSeed;

            IRandomizer randomizer = _gameSettings.Rules.General.RandomizerType switch
            {
                RandomizerType.SevenBag => new CountPerBagRandomizer(_availablePieces.Keys, seed),
                RandomizerType.FourteenBag => new CountPerBagRandomizer(_availablePieces.Keys, seed, 2),
                RandomizerType.Stride => new StrideRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Random => new RandomRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Classic => new ClassicRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Pairs => new PairsRandomizer(_availablePieces.Keys, seed),
                RandomizerType.Custom => new CustomRandomizer(
                    _availablePieces.Keys,
                    _gameSettings.Rules.General.CustomRandomizerScript,
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

            if (_isRestarting) return;
            
            _spawner.SetAvailablePieces(_availablePieces);
        }

        private void InitializePieceContainers()
        {
            var previewContainers = new List<PieceContainer>();
            for (var i = 0; i < _gameSettings.Rules.General.NextPieceCount; i++)
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
            if (!_gameSettings.Rules.Controls.AllowHold) return;
            var pieceHolder = Object.Instantiate(_pieceContainerPrefab, _board.transform);
            pieceHolder.transform.localPosition = new Vector3(
                -PieceContainer.Width,
                (int) _board.Height - PieceContainer.Height
            );

            _inputProcessor.PieceHolder = pieceHolder;

        }
    }
}