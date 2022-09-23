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
            if (!_gameSettings.Rules.Controls.OverrideHandling)
                _gameSettings.Rules.Controls.Handling = AppSettings.Handling with {};
        }
    }
}