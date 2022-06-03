using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record GravityChangedMessage : Message
    {
        public double Gravity;
    }
}