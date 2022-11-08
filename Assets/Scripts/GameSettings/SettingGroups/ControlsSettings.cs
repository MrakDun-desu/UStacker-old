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
        private RotationSystemType _rotationSystemType = RotationSystemType.SRS;
        private string _customRotationSystem = string.Empty;
        
        public bool Allow180Spins = true;
        public bool AllowHardDrop = true;
        public bool AllowHold = true;
        public bool UnlimitedHold;
        public bool ShowGhostPiece = true;

        [Tooltip("Changes how the pieces spawn, how they rotate and kick and which spins are treated as full spins")]
        public RotationSystemType RotationSystemType
        {
            get => _rotationSystemType;
            set
            {
                _rotationSystemType = value;
                LoadCustomSystemIfNeeded();
            }
        }

        [Tooltip("Filename of the custom rotation system")]
        public string CustomRotationSystem
        {
            get => _customRotationSystem;
            set
            {
                _customRotationSystem = value;
                LoadCustomSystemIfNeeded();
            }
        }
        
        [Tooltip("If set, this handling will override the global handling")]
        public bool OverrideHandling;
        public HandlingSettings Handling = new();
        
        // not shown in the settings UI
        public RotationSystem ActiveRotationSystem;

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