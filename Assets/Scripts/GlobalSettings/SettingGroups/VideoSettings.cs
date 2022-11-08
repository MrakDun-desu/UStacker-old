using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record VideoSettings
    {
        // backing fields
        private float _backgroundVisibility = .6f;

        public FullScreenMode FullscreenMode { get; set; } = FullScreenMode.ExclusiveFullScreen;
        public Resolution Resolution { get; set; } = new();

        public float BackgroundVisibility
        {
            get => _backgroundVisibility;
            set => _backgroundVisibility = Mathf.Clamp(value, 0, 1);
        }

        public bool UseVsync { get; set; }
    }
}