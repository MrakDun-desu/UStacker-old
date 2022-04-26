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
        private static string RandomizersPath => Path.Combine(Application.persistentDataPath, "ruleCustomization/randomizers");
        private readonly int _pieceCount;
        private readonly PieceSpawner _spawner;
        private readonly Piece[] _availablePieces;
        
        public RulesGeneralInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings,
            int pieceCount,
            PieceSpawner spawner,
            Piece[] availablePieces) : base(problemBuilder, gameSettings)
        {
            _pieceCount = pieceCount;
            _spawner = spawner;
            _availablePieces = availablePieces;
        }

        public override void Execute()
        {
            InitializeSeed();
            InitializeRandomizer();
        }

        private void InitializeSeed()
        {
            var newSeed = _gameSettings.Rules.General.UseRandomSeed ? 
                Random.Range(int.MinValue, int.MaxValue) : _gameSettings.Rules.General.SpecificSeed;
            _gameSettings.Rules.General.ActiveSeed = newSeed;
            Random.InitState(newSeed);
        }

        private void InitializeRandomizer()
        {
            if (_gameSettings.Rules.General.RandomizerType == RandomizerType.Custom) {
                var randomizerScriptPath = Path.Combine(RandomizersPath, _gameSettings.Rules.General.CustomRandomizerName);
                if (!File.Exists(randomizerScriptPath)) {
                    _errorBuilder.AppendLine("Custom randomizer script not found.");
                    return;
                }
                _gameSettings.Rules.General.CustomRandomizerScript = File.ReadAllText(randomizerScriptPath);
            }

            var isValid = true;

            IRandomizer randomizer = _gameSettings.Rules.General.RandomizerType switch
            {
                RandomizerType.SevenBag => new CountPerBagRandomizer(_pieceCount),
                RandomizerType.FourteenBag => new CountPerBagRandomizer(_pieceCount, 2),
                RandomizerType.Random => new RandomRandomizer(_pieceCount),
                RandomizerType.Classic => new ClassicRandomizer(_pieceCount),
                RandomizerType.Pairs => new PairsRandomizer(_pieceCount),
                RandomizerType.Custom => new CustomRandomizer(
                    _pieceCount,
                    _gameSettings.Rules.General.CustomRandomizerScript,
                    _gameSettings.Rules.General.ActiveSeed,
                    out isValid),
                _ => new CountPerBagRandomizer(_pieceCount),
            };

            if (!isValid) {
                _errorBuilder.AppendLine("Custom random bag script is not valid.");
                return;
            }

            _spawner.Randomizer = randomizer;
            _spawner.AvailablePieces = _availablePieces;
        }
    }
}