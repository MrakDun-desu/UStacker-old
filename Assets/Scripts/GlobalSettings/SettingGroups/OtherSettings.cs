using System;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record OtherSettings
    {
        public bool UseDiscordRichPresence { get; set; } = true;
    }
}