using System.Collections;
using System.Collections.Generic;
using Blockstacker.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Startup
{
    public class StartupHandler : MonoBehaviour
    {
        private void Awake()
        {
            AppSettings.Load();
            SceneManager.LoadScene("Scene_Menu");
        }
    }
}
