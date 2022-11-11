using System;
using System.IO;
using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings.Groups;
using Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record ControlsSettings
    {
        // backing fields
        [SerializeField]
        private RotationSystemType _rotationSystemType = RotationSystemType.SRS;
        [SerializeField]
        private string _customRotationSystem = string.Empty;

        [field: SerializeField]
        public bool Allow180Spins { get; set; } = true;
        [field: SerializeField]
        public bool AllowHardDrop { get; set; } = true;
        [field: SerializeField]
        public bool AllowHold { get; set; } = true;
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
        public RotationSystem ActiveRotationSystem { get; set; }

        private bool TryReloadRotationSystem()
        {
            const string filenameExtension = ".json";
            var filePath = Path.Combine(CustomizationPaths.RotationSystems,
                CustomRotationSystem + filenameExtension);

            if (!File.Exists(filePath)) return false;

            ActiveRotationSystem =
                JsonConvert.DeserializeObject<RotationSystem>(File.ReadAllText(filePath), StaticSettings.JsonSerializerSettings);

            return true;
        }

        private void LoadCustomSystemIfNeeded()
        {
            if (_rotationSystemType != RotationSystemType.Custom ||
                string.IsNullOrEmpty(_customRotationSystem))
                return;

            if (TryReloadRotationSystem())
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom rotation system load failed!",
                        $"Rotation system {CustomRotationSystem} couldn't be found.",
                        AlertType.Error));
            }
            else
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom rotation system loaded!",
                        $"Rotation system {CustomRotationSystem} was loaded into game settings.",
                        AlertType.Success));
            }
        }
    }
}