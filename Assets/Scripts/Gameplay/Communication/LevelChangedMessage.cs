using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LevelChangedMessage : MidgameMessage
    {
        public readonly string Level;

        public LevelChangedMessage(string level, double time) : base(time)
        {
            Level = level;
        }
    }
}