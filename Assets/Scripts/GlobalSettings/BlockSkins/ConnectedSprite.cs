using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.Enums;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public class ConnectedSprite
    {
        public Edges Edges;
        public List<SpriteRecord> Sprites = new();
    }
}