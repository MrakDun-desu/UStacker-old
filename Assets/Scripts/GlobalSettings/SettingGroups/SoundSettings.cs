using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.Music;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record SoundSettings
    {
        private float _masterVolume = .5f;
        private float _musicVolume = 1;
        private float _effectsVolume = 1;
        private float _menuSoundsVolume = 1;

        public float MasterVolume
        {
            get => _masterVolume;
            set => _masterVolume = Mathf.Clamp(value, 0, 1);
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set => _musicVolume = Mathf.Clamp(value, 0, 1);
        }

        public float EffectsVolume
        {
            get => _effectsVolume;
            set => _effectsVolume = Mathf.Clamp(value, 0, 1);
        }

        public float MenuSoundsVolume
        {
            get => _menuSoundsVolume;
            set => _menuSoundsVolume = Mathf.Clamp(value, 0, 1);
        }

        public bool MuteWhenOutOfFocus { get; set; }

        public bool HearNextPieces { get; set; }

        // not viewed in global settings UI
        public Dictionary<string, MusicOption> GameMusicDictionary { get; set; } = new();
    }
}