using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettingsLevelling
    {
        public float Gravity = .02f;
        public float LockDelay = .5f;
        public uint StartingLevel = 1;
        public LevellingSystem LevellingSystem = LevellingSystem.None;
        public string CustomLevellingScriptName = "";
        public string CustomLevellingScript = "";
    }
}