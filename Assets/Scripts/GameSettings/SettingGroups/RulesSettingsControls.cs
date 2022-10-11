using System;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings.Groups;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsControls
    {
        public bool Allow180Spins = true;
        public bool AllowHardDrop = true;
        public bool AllowHold = true;
        public bool UnlimitedHold;
        public bool ShowGhostPiece = true;
        
        [Tooltip("Changes how the pieces spawn, how they rotate and kick and which spins are treated as full spins")]
        public RotationSystemType RotationSystem = RotationSystemType.SRS;
        
        [Tooltip("Filename of the custom rotation system")]
        public string CustomRotationSystemName = "";
        
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
    }
}