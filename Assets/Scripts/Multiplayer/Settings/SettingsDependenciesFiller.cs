
/************************************
SettingsDependenciesFiller.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Multiplayer.Settings
{
    public class SettingsDependenciesFiller : MonoBehaviour
    {
        private IGameSettingsDependency[] _dependencies;

        private void Awake()
        {
            _dependencies = GetComponentsInChildren<IGameSettingsDependency>();
        }

        public void SetGameSettings(GameSettingsSO.SettingsContainer gameSettings)
        {
            foreach (var dependency in _dependencies)
                dependency.GameSettings = gameSettings;
        }
    }
}
/************************************
end SettingsDependenciesFiller.cs
*************************************/
