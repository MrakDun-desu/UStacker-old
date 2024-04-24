
/************************************
SkinRecord.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using UStacker.GlobalSettings.Enums;

namespace UStacker.GlobalSettings.BlockSkins
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
        public Dictionary<Edges, List<SpriteRecord>> ConnectedSprites = new();
    }
}
/************************************
end SkinRecord.cs
*************************************/
