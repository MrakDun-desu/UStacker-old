using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LevelChangedMessage : MidgameMessage
    {
        public readonly uint Level;

        public LevelChangedMessage(uint level, double time) : base(time)
        {
            Level = level;
        }
    }
}