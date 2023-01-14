using System;
using System.IO;
using UStacker.Common;
using UStacker.Common.Alerts;
using UStacker.GameSettings.Enums;
using UnityEngine;

namespace UStacker.GameSettings.SettingGroups
{
    [Serializable]
    public record GeneralSettings
    {
        // backing fields
        [SerializeField]
        private uint _nextPieceCount = 5;
        [SerializeField]
        private RandomizerType _randomizerType = RandomizerType.SevenBag;
        [SerializeField]
        private string _customRandomizerName = string.Empty;

        [field: SerializeField]
        public AllowedSpins AllowedSpins { get; set; } = AllowedSpins.TSpins;
        [field: SerializeField]
        public bool UseCustomSeed { get; set; }
        [field: SerializeField]
        public ulong CustomSeed { get; set; }
        [field: SerializeField]
        public string CustomRandomizerScript { get; set; } = "";
        [field: SerializeField]
        public ulong ActiveSeed { get; set; }

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

        public uint NextPieceCount
        {
            get => _nextPieceCount;
            set => _nextPieceCount = Math.Min(value, 6);
        }

        private bool TryReloadRandomizer()
        {
            const string filenameExtension = ".lua";
            var filePath = Path.Combine(PersistentPaths.Randomizers,
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

            var shownAlert = TryReloadRandomizer()
                ? new Alert("Custom randomizer loaded!",
                    $"Randomizer {CustomRandomizer} was loaded into game settings.",
                    AlertType.Success)
                : new Alert("Custom randomizer load failed!",
                    $"Randomizer {CustomRandomizer} couldn't be found.",
                    AlertType.Error);

            AlertDisplayer.Instance.ShowAlert(shownAlert);
        }
    }
}