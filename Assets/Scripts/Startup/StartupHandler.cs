using System;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Changers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Blockstacker.Startup
{
    public class StartupHandler : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private InputActionAsset _actionAsset;
        public event Action SettingChanged;
        private void Start()
        {
            AddSceneChangeMethods();
            AppSettings.TryLoad();
            SettingChanged?.Invoke();
            SceneManager.LoadScene("Scene_Menu");
        }

        private void AddSceneChangeMethods()
        {
            SceneManager.sceneLoaded += (_, _) => _actionAsset.LoadBindingOverridesFromJson(AppSettings.Rebinds);
            SceneManager.sceneLoaded += (_, _) => AppSettings.TrySave();
        }
    }
}
