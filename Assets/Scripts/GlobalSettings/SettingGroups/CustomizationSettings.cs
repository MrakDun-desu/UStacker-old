using System;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record CustomizationSettings
    {
        public string SkinFolder = "";
        public string SoundPackFolder = "";
        public string BackgroundFolder = "";
    }
}