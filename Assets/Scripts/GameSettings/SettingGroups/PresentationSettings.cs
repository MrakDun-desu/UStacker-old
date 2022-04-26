using System;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class PresentationSettings
    {
        public string Title = "Custom game";
        public bool UseCountdown = true;
        public float CountdownInterval = 1;
        public uint CountdownCount = 3;
    }
}