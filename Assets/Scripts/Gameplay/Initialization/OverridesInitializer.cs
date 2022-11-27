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

            if (overrides.StartingLevel is not null)
                _gameSettings.Objective.StartingLevel = overrides.StartingLevel;
            if (overrides.CountdownCount is {} count)
                _gameSettings.Presentation.CountdownCount = count;
            if (overrides.CountdownCount is {} interval)
                _gameSettings.Presentation.CountdownInterval = interval;
        }
    }
}