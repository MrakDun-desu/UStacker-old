using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record GravityChangedMessage : MidgameMessage
    {
        public double Gravity;
    }
}