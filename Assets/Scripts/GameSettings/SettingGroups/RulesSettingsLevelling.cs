using System;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettingsLevelling
    {
        public float Gravity = .02f;
        public float LockDelay = .5f;
        public bool UseLevelling = true;
        public bool UseMasterLevels = true;
        public uint StartingLevel = 1;
        public bool UseCustomLevellingScript = false;
        public string CustomLevellingScriptName = "";
        public string CustomLevellingScript = "";
    }
}