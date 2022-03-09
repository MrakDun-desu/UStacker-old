using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Common
{
    public class Navigator : MonoSingleton
    {
        public void LoadMenu() => SceneManager.LoadScene("Scene_Menu");
        public void LoadSettings() => SceneManager.LoadScene("Scene_GlobalSettings");
        public void LoadGameSettings() => SceneManager.LoadScene("Scene_GameSettings");
        public void LoadGame() => SceneManager.LoadScene("Scene_GameCustom");
        public void EndGame() => Application.Quit();
    }
}