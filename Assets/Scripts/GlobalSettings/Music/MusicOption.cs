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
        public string Name { get; } = Name;
    }

    public enum OptionType : byte
    {
        Track,
        Group,
        Random
    }
}