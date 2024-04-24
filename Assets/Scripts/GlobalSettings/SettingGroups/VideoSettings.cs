
/************************************
VideoSettings.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record VideoSettings
    {
        private float _backgroundVisibility = .6f;
        private float _targetFramerate = float.PositiveInfinity;

        public FullScreenMode FullscreenMode { get; set; } = FullScreenMode.ExclusiveFullScreen;
        public Resolution Resolution { get; set; } = new();

        public float BackgroundVisibility
        {
            get => _backgroundVisibility;
            set => _backgroundVisibility = Mathf.Clamp(value, 0, 1);
        }

        public float TargetFramerate
        {
            get => _targetFramerate;
            set => _targetFramerate = Mathf.Max(value, 1);
        }
    }
}
/************************************
end VideoSettings.cs
*************************************/
