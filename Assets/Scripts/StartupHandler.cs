using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Startup
{
    public class StartupHandler : MonoBehaviour
    {
        private void Awake() => SceneManager.LoadScene("Scene_Menu");
    }
}
