using System;

namespace Blockstacker.Music
{
    [Serializable]
    public record MusicEntry
    {
        public string TrackName;
        public uint TrackFrequency = 1u;
    }
}