using System;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record OtherSettings
    {
        public bool UseDiscordRichPresence { get; set; } = true;
    }
}