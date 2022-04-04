using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettingsGeneral
    {
        public RandomBagType RandomBagType = RandomBagType.SevenBag;
        public string RandomBagName = "";
        public string RandomBagScript = "";
        public AllowedSpins AllowedSpins = AllowedSpins.TSpins;
        public bool UseRandomSeed = true;
        public int SpecificSeed = 0;
        public int ActualSeed = 0;
        public uint NextPieceCount = 5;
    }
}