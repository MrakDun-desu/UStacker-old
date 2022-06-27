using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record ScoreAddedMessage : MidgameMessage
    {
        public long Score;
    }
}