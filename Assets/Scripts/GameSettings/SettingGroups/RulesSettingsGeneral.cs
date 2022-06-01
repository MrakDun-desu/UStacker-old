using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsGeneral
    {
        public RandomizerType RandomizerType = RandomizerType.SevenBag;
        public string CustomRandomizerName = "";
        public string CustomRandomizerScript = "";
        public AllowedSpins AllowedSpins = AllowedSpins.TSpins;
        public bool UseRandomSeed = true;
        public int SpecificSeed;
        public int ActiveSeed;
        public uint NextPieceCount = 5;
    }
}