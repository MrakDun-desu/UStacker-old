using System;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettings
    {
        public RulesSettingsGeneral General = new();
        public RulesSettingsControls Controls = new();
        public RulesSettingsLevelling Levelling = new();
        public RulesSettingsBoardDimensions BoardDimensions = new();
        public RulesSettingsHandling Handling = new();

    }
}