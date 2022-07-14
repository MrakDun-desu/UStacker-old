using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public record SpriteRecord
    {
        public uint PixelsPerUnit;
        public Vector2Int PivotPoint;
        public Vector2Int SpriteStart;
        public Vector2Int SpriteSize;

        public Sprite Sprite;
    }
}