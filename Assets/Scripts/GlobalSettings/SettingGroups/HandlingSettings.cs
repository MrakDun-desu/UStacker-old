using System;
using System.ComponentModel;
using Blockstacker.Common.Attributes;
using Blockstacker.GlobalSettings.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record HandlingSettings
    {
        [Tooltip("Time before automatic movement left or right is activated")]
        [MinRestraint(1/60d, true)]
        [MaxRestraint(2/6d, false)]
        [Description("Delayed Auto Shift")]
        public double DelayedAutoShift = .125d;
        
        [Tooltip("Time between automatic movements left or right")]
        [MinRestraint(0, true)]
        [MaxRestraint(5/60d, false)]
        [Description("Automatic Repeat Rate")]
        public double AutomaticRepeatRate;
        
        [Tooltip("How many times Soft Drop increases fall speed")]
        [MinRestraint(2, true)]
        [MaxRestraint(41, false)]
        [Description("Soft Drop Factor")]
        public double SoftDropFactor = double.PositiveInfinity;
        
        [Tooltip("How long to delay DAS when delaying")]
        [MinRestraint(0, true)]
        [MaxRestraint(2/6d, false)]
        [Description("DAS Cut Delay")]
        public double DasCutDelay;
        
        [Tooltip("After which actions to delay DAS")]
        [Description("Delay DAS on")]
        public DelayDasOn DelayDasOn = DelayDasOn.Nothing;
        
        [Tooltip("How long to disable hard drop for after dropping a piece")]
        [MinRestraint(0, true)]
        [MaxRestraint(5/60d, false)]
        [Description("Double Drop Prevention Interval")]
        public double DoubleDropPreventionInterval;

        [FormerlySerializedAs("AntiDasBehavior")]
        [Tooltip("What to do when DAS is activated in 2 directions")]
        [Description("Simultaneous DAS Behavior")]
        public SimultaneousDasBehavior SimultaneousDasBehavior = SimultaneousDasBehavior.CancelFirstDirection;
        
        [Tooltip("What to do when piece is moving both vertically and diagonally")]
        [Description("Diagonal Lock Behavior")]
        public DiagonalLockBehavior DiagonalLockBehavior = DiagonalLockBehavior.DontLock;
        
        [Tooltip("If turned on, will cancel DAS delay with a piece movement")]
        [Description("Cancel DAS Delay With Movement")]
        public bool CancelDelayWithMovement = true;
        
    }
}