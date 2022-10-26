using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Common
{
    public class Navigator : MonoBehaviour
    {
        private const string MAIN_MENU_SCENE ="Scene_Menu_Main";
        private const string GAME_SETTINGS_SCENE = "Scene_Menu_GameSettings";
        private const string GAME_CUSTOM_SCENE = "Scene_Game_Custom";
        
        public void LoadMenu()
        {
            SceneManager.LoadScene(MAIN_MENU_SCENE);
        }

        public void LoadGameSettings()
        {
            SceneManager.LoadScene(GAME_SETTINGS_SCENE);
        }

        public void LoadGame()
        {
            SceneManager.LoadScene(GAME_CUSTOM_SCENE);
        }

        public void EndGame()
        {
            Application.Quit();
        }

        [UsedImplicitly]
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}