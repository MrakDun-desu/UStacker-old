using System;
using UnityEngine;

namespace Blockstacker.Settings.Groups
{
    [Serializable]
    public class VideoSettings
    {
        public string FullscreenMode = "";
        public Vector2Int Resolution;
        public int RefreshRate;
        public float BackgroundVisibility = .6f;
        public float ParticleCount = 1;
        public bool UseVsync = false;
    }
}