using System;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsBoardDimensions
    {
        [MinRestraint(2, true)]
        [MaxRestraint(500, true)]
        public uint BoardHeight = 22;
        
        [MinRestraint(4, true)]
        [MaxRestraint(500, true)]
        public uint BoardWidth = 10;
        
        [Tooltip("Importance changes depending on the Topout Condition")]
        public uint LethalHeight = 20;
        
        [MinRestraint(2, true)]
        [MaxRestraint(600, true)]
        public uint PieceSpawnHeight = 21;
        
        [Tooltip("Defines how the player loses the game")]
        public TopoutCondition TopoutCondition = TopoutCondition.AllBlocksAboveLethal;
        
        [Tooltip("If the player clears a line, some Topout Conditions are ignored")]
        public bool AllowClutchClears = true;
        
        [MinRestraint(2, true)]
        [MaxRestraint(200, true)]
        [Tooltip("Space above and below the board in units. Applies for 100% Board Zoom")]
        public uint BoardPadding = 4;
    }
}