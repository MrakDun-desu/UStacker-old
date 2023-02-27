using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UStacker.Gameplay.GameStateManagement;

namespace UStacker.Gameplay
{
    public class GameOpener : MonoBehaviour
    {
        public void OpenGame(GameSettingsSO gameSettingsAsset)
        {
            GameInitializer.Replay = null;
            GameInitializer.GameType = gameSettingsAsset.GameType.Value;
            GameInitializer.GameSettings = gameSettingsAsset.Settings;
            GameStateManager.IsReplay = false;
            SceneManager.LoadScene("Scene_Game_Singleplayer");
        }
    }
}