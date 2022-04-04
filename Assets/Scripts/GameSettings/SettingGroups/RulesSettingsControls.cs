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
        public KickTableType KickTable = KickTableType.SRS;
        public string CustomKickTableName = "";
        public KickSystem CustomKickTable;
        public float PiecePlacedDelay = 0;
        public float LineClearDelay = 0;
        public OnTouchGround OnTouchGround = OnTouchGround.LimitedTime;
        public float OnTouchGroundAmount = 5;
        public bool ShowGhostPiece = true;
    }
}