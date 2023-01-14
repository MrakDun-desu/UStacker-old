using System;
using System.IO;
using UStacker.Common;
using UStacker.Common.Alerts;
using UStacker.GameSettings.Enums;
using UStacker.GlobalSettings.Groups;
using Newtonsoft.Json;
using UnityEngine;

namespace UStacker.GameSettings.SettingGroups
{
    [Serializable]
    public record ControlsSettings
    {
        // backing fields
        [SerializeField]
        private RotationSystemType _rotationSystemType = RotationSystemType.SRSPlus;
        [SerializeField]
        private string _customRotationSystem = string.Empty;

        [field: SerializeField]
        public bool Allow180Spins { get; set; } = true;
        [field: SerializeField]
        public bool AllowHardDrop { get; set; } = true;
        [field: SerializeField]
        public bool AllowHold { get; set; } = true;
        [field: SerializeField] 
        public bool AllowMoveToWall { get; set; } = true;
        [field: SerializeField]
        public bool UnlimitedHold { get; set; }
        [field: SerializeField]
        public bool ShowGhostPiece { get; set; } = true;
        [field: SerializeField]
        public bool OverrideHandling { get; set; }
        [field: SerializeField]
        public HandlingSettings Handling { get; set; } = new();

        public RotationSystemType RotationSystemType
        {
            get => _rotationSystemType;
            set
            {
                _rotationSystemType = value;
                LoadCustomSystemIfNeeded();
            }
        }

        public string CustomRotationSystem
        {
            get => _customRotationSystem;
            set
            {
                _customRotationSystem = value;
                LoadCustomSystemIfNeeded();
            }
        }

        // not shown in the settings UI
        [field: SerializeField]
        public RotationSystem ActiveRotationSystem { get; set; }

        private bool TryReloadRotationSystem()
        {
            const string filenameExtension = ".json";
            var filePath = Path.Combine(PersistentPaths.RotationSystems,
                CustomRotationSystem + filenameExtension);

            if (!File.Exists(filePath)) return false;

            ActiveRotationSystem =
                JsonConvert.DeserializeObject<RotationSystem>(File.ReadAllText(filePath), StaticSettings.DefaultSerializerSettings);

            return true;
        }

        private void LoadCustomSystemIfNeeded()
        {
            if (_rotationSystemType != RotationSystemType.Custom ||
                string.IsNullOrEmpty(_customRotationSystem))
                return;

            var shownAlert = TryReloadRotationSystem()
                ? new Alert("Custom rotation system loaded!",
                    $"Rotation system {CustomRotationSystem} was loaded into game settings.",
                    AlertType.Success)
                : new Alert("Custom rotation system load failed!",
                    $"Rotation system {CustomRotationSystem} couldn't be found.",
                    AlertType.Error);

            _ = AlertDisplayer.Instance.ShowAlert(shownAlert);
        }
    }
}