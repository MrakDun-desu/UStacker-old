using System;
using Blockstacker.GameSettings.Enums;

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
        public float PiecePlacedDelay = 0;
        public float LineClearDelay = 0;
        public OnTouchGround OnTouchGround = OnTouchGround.LimitedTime;
        public float OnTouchGroundAmount = 5;
        public bool ShowGhostPiece = true;
    }
}