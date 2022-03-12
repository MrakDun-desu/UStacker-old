using System;

namespace Blockstacker.Settings.Groups
{
    [Serializable]
    public class SoundSettings
    {
        public float MasterVolume = 1;
        public float MusicVolume = 1;
        public float EffectsVolume = 1;
        public float MenuSoundsVolume = 1;
        public bool MuteWhenOutOfFocus = false;
    }
}