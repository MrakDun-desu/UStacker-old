using System;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings.Groups;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettingsControls
    {
        public bool Allow180Spins = true;
        public bool AllowHardDrop = true;
        public bool AllowHold = true;
        public bool UnlimitedHold = false;
        public RotationSystemType RotationSystem = RotationSystemType.SRS;
        public string CustomRotationSystemName = "";
        public RotationSystem ActiveRotationSystem;
        public double PiecePlacedDelay = 0;
        public double LineClearDelay = 0;
        public OnTouchGround OnTouchGround = OnTouchGround.LimitedTime;
        public double OnTouchGroundAmount = 5;
        public bool ShowGhostPiece = true;
        public bool OverrideHandling = false;
        public HandlingSettings Handling = new();
    }
}