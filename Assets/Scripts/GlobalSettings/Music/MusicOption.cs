
/************************************
MusicOption.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Music
{
    [Serializable]
    public record MusicOption(OptionType OptionType, string Name)
    {
        public MusicOption() : this(OptionType.Random, "")
        {
        }

        public OptionType OptionType { get; set; } = OptionType;
        public string Name { get; set; } = Name;
    }

    public enum OptionType : byte
    {
        Track,
        Group,
        Random
    }
}
/************************************
end MusicOption.cs
*************************************/
