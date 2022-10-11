using System;
using System.ComponentModel;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsGeneral
    {
        [Tooltip("Will determine how the pieces will spawn")]
        public RandomizerType RandomizerType = RandomizerType.SevenBag;
        
        [Tooltip("Filename of the custom randomizer")]
        public string CustomRandomizerName = "";
        
        [Tooltip("Which spins will be significant for score. Also changes messages sent to user scripts")]
        public AllowedSpins AllowedSpins = AllowedSpins.TSpins;
        
        [Tooltip("If set, random seed is generated at the start of each game. Overrides Specific Seed")]
        public bool UseRandomSeed = true;
        
        [Tooltip("Will be used every time when the game starts")]
        [Description("Seed")]
        public int SpecificSeed;
        
        [Tooltip("How many piece previews will be shown")]
        [MinRestraint(0, true)]
        [MaxRestraint(7, true)]
        public uint NextPieceCount = 5;
        
        // not shown in the settings UI
        public string CustomRandomizerScript = "";
        public int ActiveSeed;
    }
}