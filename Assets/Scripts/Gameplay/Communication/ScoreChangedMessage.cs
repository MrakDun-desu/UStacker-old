using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record ScoreChangedMessage : Message
    {
        public long Score;
    }
}