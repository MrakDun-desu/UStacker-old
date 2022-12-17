using System;
using System.Collections.Generic;
using UStacker.GlobalSettings.Enums;

namespace UStacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public class ConnectedSprite
    {
        public Edges Edges;
        public List<SpriteRecord> Sprites = new();
    }
}