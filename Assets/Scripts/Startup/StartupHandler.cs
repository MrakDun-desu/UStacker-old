using System;
using Blockstacker.Settings;
using Blockstacker.Settings.Changers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Startup
{
    public class StartupHandler : MonoBehaviour, ISettingChanger
    {
        public event Action SettingChanged;
        private void Awake()
        {
            AppSettings.Load();
            SettingChanged?.Invoke();
            SceneManager.LoadScene("Scene_Menu");
        }
    }
}
