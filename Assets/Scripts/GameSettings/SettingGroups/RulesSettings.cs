using System;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettings
    {
        public RulesSettingsGeneral General = new();
        public RulesSettingsControls Controls = new();
        public RulesSettingsGravity Gravity = new();
        public RulesSettingsBoardDimensions BoardDimensions = new();
    }
}