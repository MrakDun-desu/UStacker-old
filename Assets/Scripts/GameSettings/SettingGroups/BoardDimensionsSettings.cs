using System;
using Blockstacker.Common.Attributes;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record BoardDimensionsSettings
    {
        [MinRestraint(4, true)]
        [MaxRestraint(500, true)]
        public uint BoardWidth = 10;
        
        [MinRestraint(2, true)]
        [MaxRestraint(500, true)]
        public uint BoardHeight = 22;
        
        [Tooltip("Importance changes depending on the Topout Condition")]
        public uint LethalHeight = 20;
        
        [MinRestraint(2, true)]
        [MaxRestraint(600, true)]
        public uint PieceSpawnHeight = 21;
        
        [Tooltip("If blocks are placed higher than this height, they are cleared from the board after placement")]
        public uint BlockCutHeight = 40;
        
        [MinRestraint(2, true)]
        [MaxRestraint(200, true)]
        [Tooltip("Space above and below the board in units. Applies for 100% Board Zoom")]
        public uint BoardPadding = 4;
    }
}