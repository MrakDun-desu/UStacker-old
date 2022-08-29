using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.Music;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record SoundSettings
    {
        public float MasterVolume = 0.5f;
        public float MusicVolume = 1;
        public float EffectsVolume = 1;
        public float MenuSoundsVolume = 1;
        public bool MuteWhenOutOfFocus;
        public bool HearNextPieces;
        
        // not viewed in global settings UI
        public Dictionary<string, MusicOption> GameMusicDictionary = new();
    }
}