
/************************************
GravityChangedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct GravityChangedMessage : IMidgameMessage
    {
        public readonly double Gravity;

        public GravityChangedMessage(double gravity, double time)
        {
            Time = time;
            Gravity = gravity;
        }

        public double Time { get; }
    }
}
/************************************
end GravityChangedMessage.cs
*************************************/
