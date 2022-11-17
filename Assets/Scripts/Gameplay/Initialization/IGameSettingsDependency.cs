using Blockstacker.GameSettings;

namespace Blockstacker.Gameplay.Initialization
{
    public interface IGameSettingsDependency
    {
        GameSettingsSO GameSettings { set; }
    }
}