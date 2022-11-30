using System.Text;
using Blockstacker.GameSettings;

namespace Blockstacker.Gameplay.Initialization
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