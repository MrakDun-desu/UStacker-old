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
        private uint _nextPieceCount = 5;
        private RandomizerType _randomizerType = RandomizerType.SevenBag;
        private string _customRandomizerName = string.Empty;

        public RandomizerType RandomizerType
        {
            get => _randomizerType;
            set
            {
                _randomizerType = value;
                ReloadRandomizerIfNeeded();
            }
        }

        public string CustomRandomizer
        {
            get => _customRandomizerName;
            set
            {
                _customRandomizerName = value;
                ReloadRandomizerIfNeeded();
            }
        }

        public AllowedSpins AllowedSpins { get; set; } = AllowedSpins.TSpins;

        public bool UseCustomSeed { get; set; }

        public int CustomSeed { get; set; }

        public uint NextPieceCount
        {
            get => _nextPieceCount;
            set => _nextPieceCount = Math.Max(value, 6);
        }

        // not shown in the settings UI
        public string CustomRandomizerScript { get; set; } = "";
        public int ActiveSeed { get; set; }

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
            if (_randomizerType != RandomizerType.Custom ||
                string.IsNullOrEmpty(_customRandomizerName))
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