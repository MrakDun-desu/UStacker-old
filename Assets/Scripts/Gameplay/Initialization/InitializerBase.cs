using System.Text;
using UStacker.GameSettings;

namespace UStacker.Gameplay.Initialization
{
    public abstract class InitializerBase
    {
        protected readonly StringBuilder _errorBuilder;
        protected readonly GameSettingsSO.SettingsContainer _gameSettings;

        protected InitializerBase(StringBuilder errorBuilder, GameSettingsSO.SettingsContainer gameSettings)
        {
            _errorBuilder = errorBuilder;
            _gameSettings = gameSettings;
        }

        public abstract void Execute();
    }
}