using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LevelChangedMessage : Message
    {
        public uint Level;
    }
}