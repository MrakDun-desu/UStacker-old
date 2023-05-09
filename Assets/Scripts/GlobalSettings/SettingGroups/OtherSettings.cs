
/************************************
OtherSettings.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record OtherSettings
    {
        public bool UseDiscordRichPresence { get; set; } = true;
    }
}
/************************************
end OtherSettings.cs
*************************************/
