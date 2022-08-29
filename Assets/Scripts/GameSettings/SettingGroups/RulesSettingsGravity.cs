using System;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsGravity
    {
        public double DefaultGravity = .02d;
        public double DefaultLockDelay = .5d;
        public string StartingLevel = "0";
    }
}