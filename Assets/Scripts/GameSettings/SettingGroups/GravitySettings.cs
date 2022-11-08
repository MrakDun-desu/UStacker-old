using System;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record GravitySettings
    {
        [Tooltip("Amount of units the piece will fall in one 60th of a second. Will be used by default if not overriden by game manager")]
        [MinRestraint(0, true)]
        [MaxRestraint(50, false)]
        public double DefaultGravity = .02d;
        
        [Tooltip("Determines how long it takes for piece to lock if it is not moved. Will be used by default if not overriden by game manager")]
        [MinRestraint(0, true)]
        [MaxRestraint(50, false)]
        public double DefaultLockDelay = .5d;
        
        [Tooltip("Determines when lock delay starts")]
        public LockDelayType LockDelayType = LockDelayType.OnIllegalMovement;
        
        [Tooltip("How long to wait in seconds before spawning a piece when piece has been placed")]
        [MinRestraint(0, true)]
        [MaxRestraint(10, true)]
        public double PiecePlacementDelay;
        
        [Tooltip("How long to wait in seconds before spawning a piece if lines have been cleared")]
        [MinRestraint(0, true)]
        [MaxRestraint(10, true)]
        public double LineClearDelay;
        
        [Tooltip("Determines when the piece will lock after starting lock delay the first time")]
        public HardLockType HardLockType = HardLockType.LimitedTime;
        
        [MinRestraint(0, true)]
        [MaxRestraint(50, true)]
        public double HardLockAmount = 5;
        
        [Tooltip("Defines how the player loses the game")]
        public TopoutCondition TopoutCondition = TopoutCondition.AllBlocksAboveLethal;
        
        [Tooltip("If the player clears a line, some Topout Conditions are ignored")]
        public bool AllowClutchClears = true;
    }
}