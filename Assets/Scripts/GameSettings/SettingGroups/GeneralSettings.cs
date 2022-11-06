using System;
using System.ComponentModel;
using System.IO;
using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record GeneralSettings
    {
        // backing fields
        private RandomizerType _randomizerType = RandomizerType.SevenBag;
        private string _customRandomizerName = string.Empty;

        [Tooltip("Will determine how the pieces will spawn")]
        public RandomizerType RandomizerType
        {
            get => _randomizerType;
            set
            {
                _randomizerType = value;
                ReloadRandomizerIfNeeded();
            }
        }

        [Tooltip("Filename of the custom randomizer")]
        public string CustomRandomizer
        {
            get => _customRandomizerName;
            set
            {
                _customRandomizerName = value;
                ReloadRandomizerIfNeeded();
            }
        }
        
        [Tooltip("Which spins will be significant for score. Also changes messages sent to user scripts")]
        public AllowedSpins AllowedSpins = AllowedSpins.TSpins;
        
        [Tooltip("If set, custom seed will be used instead of generating random seed at the start of the game")]
        public bool UseCustomSeed;
        
        [Tooltip("Will be used every time when the game starts")]
        [Description("Seed")]
        public int CustomSeed;
        
        [Tooltip("How many piece previews will be shown")]
        [MinRestraint(0, true)]
        [MaxRestraint(7, true)]
        public uint NextPieceCount = 5;
        
        // not shown in the settings UI
        public string CustomRandomizerScript = "";
        public int ActiveSeed;

        private bool TryReloadRandomizer()
        {
            const string filenameExtension = ".lua";
            var filePath = Path.Combine(CustomizationPaths.RotationSystems,
                CustomRandomizer + filenameExtension);

            if (!File.Exists(filePath)) return false;

            CustomRandomizerScript = File.ReadAllText(filePath);
            
            return true;
        }

        private void ReloadRandomizerIfNeeded()
        {
            if (_randomizerType != RandomizerType.Custom)
                return;
            
            if (TryReloadRandomizer())
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom randomizer load failed!",
                        $"Randomizer {CustomRandomizer} couldn't be found.",
                        AlertType.Error));
            }
            else
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom randomizer loaded!",
                        $"Randomizer {CustomRandomizer} was loaded into game settings.",
                        AlertType.Success));
            }
        }
    }
}