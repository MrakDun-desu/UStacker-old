using System;
using System.ComponentModel;
using Blockstacker.Common.Attributes;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record VideoSettings
    {
        public FullScreenMode FullscreenMode = FullScreenMode.ExclusiveFullScreen;
        public Resolution Resolution = new();
        
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        public float BackgroundVisibility = 1;
        
        [Description("Use VSync")]
        public bool UseVsync;
    }
}