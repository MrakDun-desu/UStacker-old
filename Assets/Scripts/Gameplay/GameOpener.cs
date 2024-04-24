
/************************************
GameOpener.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UnityEngine.SceneManagement;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Gameplay
{
    public class GameOpener : MonoBehaviour
    {
        public void OpenGame(GameSettingsSO gameSettingsAsset)
        {
            GameInitializer.Replay = null;
            GameInitializer.GameType = gameSettingsAsset.GameType.Value;
            GameInitializer.GameSettings = gameSettingsAsset.Settings;
            SceneManager.LoadScene("Scene_Game_Singleplayer");
        }
    }
}
/************************************
end GameOpener.cs
*************************************/
