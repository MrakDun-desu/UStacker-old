using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesHandlingInitializer : InitializerBase
    {
        public RulesHandlingInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings) : base(problemBuilder, gameSettings)
        {
        }

        public override void Execute()
        {
            _gameSettings.Rules.Handling.DelayedAutoShift =
                AppSettings.Handling.DelayedAutoShift;
            _gameSettings.Rules.Handling.AutomaticRepeatRate =
                AppSettings.Handling.AutomaticRepeatRate;
            _gameSettings.Rules.Handling.SoftDropFactor =
                AppSettings.Handling.SoftDropFactor;
            _gameSettings.Rules.Handling.DasCutDelay =
                AppSettings.Handling.DasCutDelay;
            _gameSettings.Rules.Handling.UseDasCancelling =
                AppSettings.Handling.UseDasCancelling;
            _gameSettings.Rules.Handling.UseDiagonalLock =
                AppSettings.Handling.UseDiagonalLock;
        }
    }
}