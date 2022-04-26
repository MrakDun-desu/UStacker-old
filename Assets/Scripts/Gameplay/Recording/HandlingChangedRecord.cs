using Blockstacker.GameSettings.SettingGroups;

namespace Blockstacker.Gameplay.Recording
{
    public struct HandlingChangedRecord : IRecord
    {
        public float Time { get; }
        public RulesSettingsHandling Handling { get; }

        public HandlingChangedRecord(float time, RulesSettingsHandling handling)
        {
            Time = time;
            Handling = handling;
        }
    }
}