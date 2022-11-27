using Blockstacker.Gameplay.Initialization;
using Blockstacker.GameSettings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.Gameplay.Gameplay
{
    public class GameOpener : MonoBehaviour
    {
        public void OpenGame(GameSettingsSO gameSettingsAsset)
        {
            GameInitializer.GameSettingsAsset = gameSettingsAsset;
            SceneManager.LoadScene("Scene_Game_Singleplayer");
        }
    }
}