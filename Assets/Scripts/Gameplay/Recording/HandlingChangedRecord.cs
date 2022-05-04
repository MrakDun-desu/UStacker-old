using Blockstacker.GameSettings.SettingGroups;
using Blockstacker.GlobalSettings.Groups;

namespace Blockstacker.Gameplay.Recording
{
    public struct HandlingChangedRecord : IRecord
    {
        public float Time { get; }
        public HandlingSettings Handling { get; }

        public HandlingChangedRecord(float time, HandlingSettings handling)
        {
            Time = time;
            Handling = handling;
        }
    }
}