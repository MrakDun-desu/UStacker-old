using JetBrains.Annotations;
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

        public void LoadGameSettings_CheeseRace20L()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_CheeseRace20L");
        }

        public void LoadGame_CheeseRace20L()
        {
            SceneManager.LoadScene("Scene_Game_CheeseRace20L");
        }

        public void LoadGameSettings_ClassicMarathon()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_ClassicMarathon");
        }

        public void LoadGame_ClassicMarathon()
        {
            SceneManager.LoadScene("Scene_Game_ClassicMarathon");
        }

        public void LoadGameSettings_Custom()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_Custom");
        }

        public void LoadGame_Custom()
        {
            SceneManager.LoadScene("Scene_Game_Custom");
        }

        public void LoadGameSettings_Marathon()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_Marathon");
        }

        public void LoadGame_Marathon()
        {
            SceneManager.LoadScene("Scene_Game_Marathon");
        }

        public void LoadGameSettings_Sprint40L()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_Sprint40L");
        }

        public void LoadGame_Sprint40L()
        {
            SceneManager.LoadScene("Scene_Game_Sprint40L");
        }

        public void LoadGameSettings_Ultra()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_Ultra");
        }

        public void LoadGame_Ultra()
        {
            SceneManager.LoadScene("Scene_Game_Ultra");
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