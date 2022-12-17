using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public record GravityChangedMessage : MidgameMessage
    {
        public readonly double Gravity;

        public GravityChangedMessage(double gravity, double time) : base(time)
        {
            Gravity = gravity;
        }
    }
}