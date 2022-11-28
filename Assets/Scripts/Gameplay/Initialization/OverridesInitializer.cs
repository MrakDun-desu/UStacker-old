using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;

namespace Blockstacker.Gameplay.Initialization
{
    public class OverridesInitializer : InitializerBase
    {
        private readonly string _gameType;

        public OverridesInitializer(
            StringBuilder errorBuilder, GameSettingsSO.SettingsContainer gameSettings,
            string gameType) : base(errorBuilder, gameSettings)
        {
            _gameType = gameType;
        }

        public override void Execute()
        {
            if (!AppSettings.GameOverrides.TryGetValue(_gameType, out var overrides)) return;

            if (overrides.StartingLevel is not null)
                _gameSettings.Objective.StartingLevel = overrides.StartingLevel;
            if (overrides.CountdownCount is { } count)
                _gameSettings.Presentation.CountdownCount = count;
            if (overrides.CountdownInterval is { } interval)
                _gameSettings.Presentation.CountdownInterval = interval;
        }
    }
}