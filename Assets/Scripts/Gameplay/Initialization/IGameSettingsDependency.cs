using UStacker.GameSettings;

namespace UStacker.Gameplay.Initialization
{
    public interface IGameSettingsDependency
    {
        GameSettingsSO.SettingsContainer GameSettings { set; }
    }
}