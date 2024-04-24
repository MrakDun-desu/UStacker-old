
/************************************
LockDelayChangedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct LockDelayChangedMessage : IMidgameMessage
    {
        public readonly double LockDelay;

        public LockDelayChangedMessage(double lockDelay, double time)
        {
            Time = time;
            LockDelay = lockDelay;
        }

        public double Time { get; }
    }
}
/************************************
end LockDelayChangedMessage.cs
*************************************/
