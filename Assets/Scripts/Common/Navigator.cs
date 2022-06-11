using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Common
{
    public class Navigator : MonoBehaviour
    {
        public void LoadMenu()
        {
            SceneManager.LoadScene("Scene_Menu_Main");
        }

        public void LoadSettings()
        {
            SceneManager.LoadScene("Scene_Menu_GlobalSettings");
        }

        public void LoadGameSettings()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings");
        }

        public void LoadGame()
        {
            SceneManager.LoadScene("Scene_Game_Custom");
        }

        public void EndGame()
        {
            Application.Quit();
        }
    }
}