using System;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public record SkinRecord
    {
        public string File;
        public string PieceType;
        public uint BlockNumber;
        public bool IsConnected;
        public ConnectedSprite[] ConnectedSprites;
        public uint Layer;
        public bool ShouldRotate;
        public uint AnimationFrame;
        public bool IsAnimated;
        public SpriteRecord SpriteRecord;
    }
}