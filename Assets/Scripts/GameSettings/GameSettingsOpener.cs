﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blockstacker.GameSettings
{
    public class GameSettingsOpener : MonoBehaviour
    {
        public void OpenGameSettings(SingleplayerGameAsset gameAsset)
        {
            GameSettingsManager.GameAsset = gameAsset;
            SceneManager.LoadScene("Scene_Menu_GameSettings_Singleplayer");
        }
    }
}