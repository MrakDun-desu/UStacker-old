using System;
using UnityEngine;

namespace Blockstacker.Settings.Groups
{
    [Serializable]
    public class VideoSettings
    {
        public string FullscreenMode = "";
        public Vector2 Resolution;
        public float BackgroundVisibility = .6f;
        public float ParticleCount = 1;
        public bool UseVsync = false;
    }
}