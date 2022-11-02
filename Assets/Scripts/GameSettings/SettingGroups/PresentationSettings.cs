using System;
using Blockstacker.Common.Attributes;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record PresentationSettings
    {
        [Tooltip("Will be displayed on top of the game to show it's type")]
        public string Title = "Custom game";
        
        [Tooltip("If set, will use countdown on game start, restart and resume")]
        public bool UseCountdown = true;
        
        [Tooltip("Interval between countdown ticks in seconds")]
        [MinRestraint(0.5, true)]
        [MaxRestraint(10, true)]
        public float CountdownInterval = 1;
        
        [Tooltip("How many times the countdown needs to tick before game start")]
        [MinRestraint(1, true)]
        [MaxRestraint(10, true)]
        public uint CountdownCount = 3;
    }
}