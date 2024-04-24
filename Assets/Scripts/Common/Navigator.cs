
/************************************
Navigator.cs -- created by Marek Dančo (xdanco00)
*************************************/
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UStacker.Common
{
    public class Navigator : MonoBehaviour
    {
        public void LoadMenu()
        {
            SceneManager.LoadScene("Scene_Menu_Main");
        }

        public void LoadCustomGameSettings()
        {
            SceneManager.LoadScene("Scene_Menu_GameSettings_Custom");
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
/************************************
end Navigator.cs
*************************************/
