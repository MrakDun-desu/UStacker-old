using System;
using System.Collections.Generic;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public class SkinRecord
    {
        public string SkinType = "i";
        public uint[] BlockNumbers =
        {
            0u,
            1u,
            2u,
            3u
        };
        public int Layer;
        public bool IsConnected;
        public bool RotateWithPiece;
        public float AnimationFps = 60f;
        public List<SpriteRecord> Sprites = new();
        public List<ConnectedSprite> ConnectedSprites = new();
    }
}