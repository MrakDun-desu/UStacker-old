
/************************************
SingleplayerGameAsset.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;

namespace UStacker.GameSettings
{
    [CreateAssetMenu(fileName = "GameAsset", menuName = "UStacker/Singleplayer game asset")]
    public class SingleplayerGameAsset : ScriptableObject
    {
        public GameSettingsSO GameSettings;
        public bool ShowStartingLevelInSettings;
        public string GameDescription;
    }
}
/************************************
end SingleplayerGameAsset.cs
*************************************/
