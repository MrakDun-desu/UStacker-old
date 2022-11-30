using Blockstacker.Gameplay.Initialization;
using Blockstacker.GameSettings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Gameplay
{
    public class GameOpener : MonoBehaviour
    {
        public void OpenGame(GameSettingsSO gameSettingsAsset)
        {
            GameInitializer.Replay = null;
            GameInitializer.GameSettings = gameSettingsAsset.Settings;
            GameInitializer.GameType = gameSettingsAsset.GameType.Value;
            GameInitializer.InitAsReplay = false;
            SceneManager.LoadScene("Scene_Game_Singleplayer");
        }
    }
}