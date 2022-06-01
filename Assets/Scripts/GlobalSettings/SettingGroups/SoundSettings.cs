using System;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record SoundSettings
    {
        public float MasterVolume = 1;
        public float MusicVolume = 1;
        public float EffectsVolume = 1;
        public float MenuSoundsVolume = 1;
        public bool MuteWhenOutOfFocus;
    }
}