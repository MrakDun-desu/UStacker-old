using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;

namespace Blockstacker.Gameplay.Initialization
{
    public class OverridesInitializer : InitializerBase
    {
        private readonly string _gameType;
        private readonly bool _isReplay;

        public OverridesInitializer(
            StringBuilder errorBuilder, GameSettingsSO.SettingsContainer gameSettings,
            string gameType,
            bool isReplay) : base(errorBuilder, gameSettings)
        {
            _gameType = gameType;
            _isReplay = isReplay;
        }

        public override void Execute()
        {
            if (!AppSettings.GameOverrides.TryGetValue(_gameType, out var overrides)) return;

            if (overrides.CountdownCount is { } count)
                _gameSettings.Presentation.CountdownCount = count;
            if (overrides.CountdownInterval is { } interval)
                _gameSettings.Presentation.CountdownInterval = interval;
            
            if (_isReplay) return;
            if (overrides.StartingLevel is not null)
                _gameSettings.Objective.StartingLevel = overrides.StartingLevel;
        }
    }
}