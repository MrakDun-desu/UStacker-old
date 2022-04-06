using System.Text;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesGeneralInitializer : InitializerBase
    {
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

        private void InitializeSeed() {
            int newSeed;
            if (_gameSettings.Rules.General.UseRandomSeed) {
                newSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
            else {
                newSeed = _gameSettings.Rules.General.SpecificSeed;
            }
            UnityEngine.Random.InitState(newSeed);
        }

        private void InitializeRandomizer() {
            IRandomizer randomizer = _gameSettings.Rules.General.RandomBagType switch
            {
                RandomBagType.SevenBag => new CountPerBagRandomizer(_pieceCount),
                RandomBagType.FourteenBag => new CountPerBagRandomizer(_pieceCount, 2),
                RandomBagType.Random => new RandomRandomizer(_pieceCount),

                _ => new RandomRandomizer(_pieceCount),
            };
            _manager.randomizer = randomizer;
        }
    }
}