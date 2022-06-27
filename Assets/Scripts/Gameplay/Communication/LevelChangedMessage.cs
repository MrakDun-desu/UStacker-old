using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LevelChangedMessage : MidgameMessage
    {
        public uint Level;
    }
}