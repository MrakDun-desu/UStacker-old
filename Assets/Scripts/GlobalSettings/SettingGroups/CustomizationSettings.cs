using System;
using Blockstacker.GlobalSettings.Enums;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record CustomizationSettings
    {
        public string SkinFolder;
        public string SoundPackFolder;
        public string BackgroundFolder;
        public string UiFolder;
    }
}