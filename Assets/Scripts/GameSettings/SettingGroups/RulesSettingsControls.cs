using System;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings.Groups;

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
        public RotationSystemType RotationSystem = RotationSystemType.SRS;
        public string CustomRotationSystemName = "";
        public RotationSystem ActiveRotationSystem;
        public double PiecePlacedDelay;
        public double LineClearDelay;
        public OnTouchGround OnTouchGround = OnTouchGround.LimitedTime;
        public double OnTouchGroundAmount = 5;
        public bool OverrideHandling;
        public HandlingSettings Handling = new();
    }
}