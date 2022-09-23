using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LockDelayChangedMessage : MidgameMessage
    {
        public readonly double LockDelay;

        public LockDelayChangedMessage(double lockDelay, double time) : base(time)
        {
            LockDelay = lockDelay;
        }
    }
}