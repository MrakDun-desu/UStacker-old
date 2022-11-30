using System;

namespace Blockstacker.GlobalSettings.Music
{
    [Serializable]
    public record MusicOption(OptionType OptionType, string Name)
    {
        public OptionType OptionType { get; set; } = OptionType;
        public string Name { get; } = Name;

        public MusicOption() : this(OptionType.Random, "")
        {
        }
    }

    public enum OptionType : byte
    {
        Track,
        Group,
        Random
    }
}