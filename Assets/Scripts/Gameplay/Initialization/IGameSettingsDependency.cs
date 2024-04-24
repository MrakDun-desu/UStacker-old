
/************************************
IGameSettingsDependency.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.GameSettings;

namespace UStacker.Gameplay.Initialization
{
    public interface IGameSettingsDependency
    {
        GameSettingsSO.SettingsContainer GameSettings { set; }
    }
}
/************************************
end IGameSettingsDependency.cs
*************************************/
