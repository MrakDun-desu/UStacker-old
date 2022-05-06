using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettingsLevelling
    {
        public double Gravity = .02f;
        public double LockDelay = .5f;
        public uint StartingLevel = 1;
        public LevellingSystem LevellingSystem = LevellingSystem.None;
        public string CustomLevellingScriptName = "";
        public string CustomLevellingScript = "";
    }
}