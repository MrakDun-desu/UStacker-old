using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.Enums;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public class ConnectedSprite
    {
        public ConnectedEdges ConnectedEdges;
        public List<SpriteRecord> Sprites = new();
    }
}