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

        public void LoadGameSettingsCustom()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_Custom");
        }

        public void LoadGameCustom()
        {
            SceneManager.LoadScene("Scene_Game_Custom");
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