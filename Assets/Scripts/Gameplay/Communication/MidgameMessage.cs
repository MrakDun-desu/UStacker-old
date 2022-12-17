using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public abstract record MidgameMessage : Message
    {
        public readonly double Time;

        protected MidgameMessage(double time)
        {
            Time = time;
        }
    }
}