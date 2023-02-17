using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct LockDelayChangedMessage : IMidgameMessage
    {
        public readonly double LockDelay;
        public double Time { get; }

        public LockDelayChangedMessage(double lockDelay, double time)
        {
            Time = time;
            LockDelay = lockDelay;
        }
    }
}