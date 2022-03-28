using System;
using Blockstacker.Settings;
using Blockstacker.Settings.Changers;
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
            AppSettings.TryLoad();
            SceneManager.sceneLoaded += (_, _) => _actionAsset.LoadBindingOverridesFromJson(AppSettings.Rebinds);
            SettingChanged?.Invoke();
            SceneManager.LoadScene("Scene_Menu");
        }
    }
}
