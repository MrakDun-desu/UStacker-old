using System.IO;
using System.Text;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesGeneralInitializer : InitializerBase
    {
        private readonly Piece[] _availablePieces;
        private readonly Board _board;
        private readonly InputProcessor _inputProcessor;
        private readonly bool _isRestarting;
        private readonly PieceContainer _pieceContainerPrefab;
        private readonly int _pieceCount;
        private readonly PieceSpawner _spawner;

        public RulesGeneralInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings,
            int pieceCount,
            PieceSpawner spawner,
            Piece[] availablePieces,
            Board board,
            PieceContainer pieceContainerPrefab,
            InputProcessor inputProcessor,
            bool isRestarting = false) : base(problemBuilder, gameSettings)
        {
            _pieceCount = pieceCount;
            _spawner = spawner;
            _availablePieces = availablePieces;
            _board = board;
            _pieceContainerPrefab = pieceContainerPrefab;
            _inputProcessor = inputProcessor;
            _isRestarting = isRestarting;
        }

        private static string RandomizersPath =>
            Path.Combine(Application.persistentDataPath, "ruleCustomization/randomizers");

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
                    Path.Combine(RandomizersPath, _gameSettings.Rules.General.CustomRandomizerName);
                if (!File.Exists(randomizerScriptPath))
                {
                    _errorBuilder.AppendLine("Custom randomizer script not found.");
                    return;
                }

                _gameSettings.Rules.General.CustomRandomizerScript = File.ReadAllText(randomizerScriptPath);
            }

            var isValid = true;
            var seed = _gameSettings.Rules.General.ActiveSeed;

            IRandomizer randomizer = _gameSettings.Rules.General.RandomizerType switch
            {
                RandomizerType.SevenBag => new CountPerBagRandomizer(_pieceCount, seed),
                RandomizerType.FourteenBag => new CountPerBagRandomizer(_pieceCount, seed, 2),
                RandomizerType.Random => new RandomRandomizer(_pieceCount, seed),
                RandomizerType.Classic => new ClassicRandomizer(_pieceCount, seed),
                RandomizerType.Pairs => new PairsRandomizer(_pieceCount, seed),
                RandomizerType.Custom => new CustomRandomizer(
                    _pieceCount,
                    _gameSettings.Rules.General.CustomRandomizerScript,
                    seed,
                    out isValid),
                _ => new CountPerBagRandomizer(_pieceCount, seed)
            };

            if (!isValid)
            {
                _errorBuilder.AppendLine("Custom random bag script is not valid.");
                return;
            }

            _spawner.Randomizer = randomizer;
            _spawner.SetAvailablePieces(_availablePieces);
        }

        private void InitializePieceContainers()
        {
            for (var i = 0; i < _gameSettings.Rules.General.NextPieceCount; i++)
            {
                var pieceContainer = Object.Instantiate(_pieceContainerPrefab, _board.transform);
                pieceContainer.transform.localPosition = new Vector3(
                    (int) _board.Width,
                    (int) _board.Height - PieceContainer.Height * (i + 1)
                );
                _spawner.PreviewContainers.Add(pieceContainer);
            }
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