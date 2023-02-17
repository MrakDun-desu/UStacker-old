using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct GravityChangedMessage : IMidgameMessage
    {
        public readonly double Gravity;
        public double Time { get; }

        public GravityChangedMessage(double gravity, double time)
        {
            Time = time;
            Gravity = gravity;
        }
    }
}