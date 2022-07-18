using System;
using System.Collections.Generic;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public class SkinRecord
    {
        public string PieceType = "IPiece";
        public uint[] BlockNumbers =
        {
            0u,
            1u,
            2u,
            3u
        };
        public List<ConnectedSprite> ConnectedSprites = new();
        public uint Layer;
        public bool IsConnected;
        public bool ShouldRotate;
        public float AnimationFps = 60f;
        public List<SpriteRecord> Sprites = new();
    }
}