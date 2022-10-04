using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record VideoSettings
    {
        public FullScreenMode FullscreenMode = FullScreenMode.FullScreenWindow;
        public Resolution Resolution = new();
        public float BackgroundVisibility = 1;
        public bool UseVsync;
    }
}