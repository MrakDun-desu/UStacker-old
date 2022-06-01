using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsLevelling
    {
        public double Gravity = .02d;
        public double LockDelay = .5d;
        public uint StartingLevel = 1;
        public LevellingSystem LevellingSystem = LevellingSystem.None;
        public string CustomLevellingScriptName = "";
        public string CustomLevellingScript = "";
    }
}