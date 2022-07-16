using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public record SkinRecord
    {
        public string PieceType;
        public uint[] BlockNumbers =
        {
            0u,
            1u,
            2u,
            3u
        };
        public List<ConnectedSprite> ConnectedSprites;
        public uint Layer;
        public bool IsConnected;
        public bool ShouldRotate;
        public List<SpriteRecord> Sprites;
    }
}