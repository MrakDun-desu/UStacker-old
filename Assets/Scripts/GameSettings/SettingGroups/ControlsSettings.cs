using System;
using System.IO;
using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Attributes;
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
        private RotationSystemType _rotationSystem = RotationSystemType.SRS;
        private string _customRotationSystemName = string.Empty;
        
        public bool Allow180Spins = true;
        public bool AllowHardDrop = true;
        public bool AllowHold = true;
        public bool UnlimitedHold;
        public bool ShowGhostPiece = true;

        [Tooltip("Changes how the pieces spawn, how they rotate and kick and which spins are treated as full spins")]
        public RotationSystemType RotationSystem
        {
            get => _rotationSystem;
            set
            {
                _rotationSystem = value;
                LoadCustomSystemIfNeeded();
            }
        }

        [Tooltip("Filename of the custom rotation system")]
        public string CustomRotationSystemName
        {
            get => _customRotationSystemName;
            set
            {
                _customRotationSystemName = value;
                LoadCustomSystemIfNeeded();
            }
        }
        
        [Tooltip("How long to wait in seconds before spawning a piece when piece has been placed")]
        [MinRestraint(0, true)]
        [MaxRestraint(10, true)]
        public double PiecePlacedDelay;
        
        [Tooltip("How long to wait in seconds before spawning a piece if lines have been cleared")]
        [MinRestraint(0, true)]
        [MaxRestraint(10, true)]
        public double LineClearDelay;
        
        [Tooltip("Determines when the piece will lock after starting lock delay the first time")]
        public HardLockType HardLockType = HardLockType.LimitedTime;
        
        [MinRestraint(0, true)]
        [MaxRestraint(50, true)]
        public double HardLockAmount = 5;
        
        [Tooltip("If set, this handling will override the global handling")]
        public bool OverrideHandling;
        public HandlingSettings Handling = new();
        
        // not shown in the settings UI
        public RotationSystem ActiveRotationSystem;

        private bool TryReloadRotationSystem()
        {
            const string filenameExtension = ".json";
            var filePath = Path.Combine(CustomizationPaths.RotationSystems,
                CustomRotationSystemName + filenameExtension);

            if (!File.Exists(filePath)) return false;
            
            ActiveRotationSystem =
                JsonConvert.DeserializeObject<RotationSystem>(File.ReadAllText(filePath), StaticSettings.JsonSerializerSettings);
            
            return true;
        }

        private void LoadCustomSystemIfNeeded()
        {
            if (_rotationSystem != RotationSystemType.Custom)
                return;
            
            if (TryReloadRotationSystem())
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom rotation system load failed!",
                        $"Rotation system {CustomRotationSystemName} couldn't be found.",
                        AlertType.Error));
            }
            else
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom rotation system loaded!",
                        $"Rotation system {CustomRotationSystemName} was loaded into game settings.",
                        AlertType.Success));
            }
        }
    }
}