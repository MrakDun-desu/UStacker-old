using System.Text;
using Blockstacker.GameSettings;

namespace Blockstacker.Gameplay.Initialization
{
    public abstract class InitializerBase
    {
        protected StringBuilder _errorBuilder;
        protected GameSettingsSO _gameSettings;
        public InitializerBase(StringBuilder errorBuilder, GameSettingsSO gameSettings)
        {
            _errorBuilder = errorBuilder;
            _gameSettings = gameSettings;
        }

        public abstract void Execute();
    }
}