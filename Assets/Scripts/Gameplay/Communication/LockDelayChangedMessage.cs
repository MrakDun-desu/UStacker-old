using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LockDelayChangedMessage : MidgameMessage
    {
        public double LockDelay;
    }
}