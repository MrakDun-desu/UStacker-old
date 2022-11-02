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
    }
}