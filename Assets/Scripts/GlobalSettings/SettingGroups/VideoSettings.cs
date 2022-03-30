using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public class VideoSettings
    {
        public string FullscreenMode = "";
        public Vector2Int Resolution = new(1920, 1080);
        public int RefreshRate;
        public float BackgroundVisibility = 1;
        public float ParticleCount = 1;
        public bool UseVsync = false;
    }
}