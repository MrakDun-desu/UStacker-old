using System.IO;
using System.Text;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesGeneralInitializer : InitializerBase
    {
        private static string RandomizersPath => Path.Combine(Application.persistentDataPath, "randomBagScripts");
        private readonly int _pieceCount;
        private readonly GameManager _manager;
        public RulesGeneralInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings,
            int pieceCount,
            GameManager manager) : base(problemBuilder, gameSettings)
        {
            _pieceCount = pieceCount;
            _manager = manager;
        }

        public override void Execute()
        {
            InitializeSeed();
            InitializeRandomizer();
        }

        private void InitializeSeed()
        {
            int newSeed;
            if (_gameSettings.Rules.General.UseRandomSeed) {
                newSeed = Random.Range(int.MinValue, int.MaxValue);
            }
            else {
                newSeed = _gameSettings.Rules.General.SpecificSeed;
            }
            Random.InitState(newSeed);
        }

        private void InitializeRandomizer()
        {
            if (_gameSettings.Rules.General.RandomBagType == RandomBagType.Custom) {
                var randomBagScriptPath = Path.Combine(RandomizersPath, _gameSettings.Rules.General.RandomBagName);
                if (!File.Exists(randomBagScriptPath)) {
                    _errorBuilder.AppendLine("Custom random bag script not found.");
                    return;
                }
                _gameSettings.Rules.General.RandomBagScript = File.ReadAllText(randomBagScriptPath);
            }

            bool isValid = true;

            IRandomizer randomizer = _gameSettings.Rules.General.RandomBagType switch
            {
                RandomBagType.SevenBag => new CountPerBagRandomizer(_pieceCount),
                RandomBagType.FourteenBag => new CountPerBagRandomizer(_pieceCount, 2),
                RandomBagType.Random => new RandomRandomizer(_pieceCount),
                RandomBagType.Classic => new ClassicRandomizer(_pieceCount),
                RandomBagType.Pairs => new PairsRandomizer(_pieceCount),
                RandomBagType.Custom => new CustomRandomizer(
                    _pieceCount,
                    _gameSettings.Rules.General.RandomBagScript,
                    _gameSettings.Rules.General.ActualSeed,
                    out isValid),
                _ => new CountPerBagRandomizer(_pieceCount),
            };

            if (!isValid) {
                _errorBuilder.AppendLine("Custom random bag script is not valid.");
                return;
            }

            _manager.randomizer = randomizer;
        }
    }
}