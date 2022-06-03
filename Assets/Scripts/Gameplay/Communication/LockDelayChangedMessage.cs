using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LockDelayChangedMessage : Message
    {
        public double LockDelay;
    }
}