
/************************************
CustomizationSettings.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record CustomizationSettings
    {
        public string SkinFolder { get; set; } = "";

        public string SoundPackFolder { get; set; } = "";

        public string BackgroundFolder { get; set; } = "";
    }
}
/************************************
end CustomizationSettings.cs
*************************************/
