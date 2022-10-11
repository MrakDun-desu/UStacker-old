using System;
using System.Collections.Generic;
using System.ComponentModel;
using Blockstacker.Common.Attributes;
using Blockstacker.GlobalSettings.Music;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record SoundSettings
    {
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        public float MasterVolume = 0.5f;
        
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        public float MusicVolume = 1;
        
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        [Description("Sound Effects Volume")]
        [Tooltip("Changes the volume of in-game sound effects")]
        public float EffectsVolume = 1;
        
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        [Tooltip("Changes the volume of menu sound effects")]
        public float MenuSoundsVolume = 1;
        
        [Tooltip("If set, will mute the game when window is of focus")]
        public bool MuteWhenOutOfFocus;
        
        [Tooltip("Will enable next piece sound effects. Works only if your sound pack supports this")]
        public bool HearNextPieces;
        
        // not viewed in global settings UI
        public Dictionary<string, MusicOption> GameMusicDictionary = new();
    }
}