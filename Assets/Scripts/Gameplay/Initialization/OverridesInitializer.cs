using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;

namespace Blockstacker.Gameplay.Initialization
{
    public class OverridesInitializer : InitializerBase
    {
        public OverridesInitializer(StringBuilder errorBuilder, GameSettingsSO gameSettings) : base(errorBuilder, gameSettings)
        {
        }

        public override void Execute()
        {
            if (!AppSettings.GameOverrides.TryGetValue(_gameSettings.GameType.Value, out var overrides)) return;

            _gameSettings.Objective.StartingLevel = overrides.StartingLevel;
            _gameSettings.Presentation.CountdownCount = overrides.CountdownCount;
            _gameSettings.Presentation.CountdownInterval = overrides.CountdownInterval;
        }
    }
}