using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "GameAsset", menuName = "Blockstacker/Singleplayer game asset")]
    public class SingleplayerGameAsset : ScriptableObject
    {
        public GameSettingsSO GameSettings;
        public bool ShowStartingLevelInSettings;
        public string GameDescription;
    }
}