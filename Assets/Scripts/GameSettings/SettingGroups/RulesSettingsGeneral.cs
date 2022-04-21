using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettingsGeneral
    {
        public RandomizerType RandomizerType = RandomizerType.SevenBag;
        public string CustomRandomizerName = "";
        public string CustomRandomizerScript = "";
        public AllowedSpins AllowedSpins = AllowedSpins.TSpins;
        public bool UseRandomSeed = true;
        public int SpecificSeed = 0;
        public int ActiveSeed = 0;
        public uint NextPieceCount = 5;
    }
}