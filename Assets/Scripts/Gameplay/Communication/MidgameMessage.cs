using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public abstract record MidgameMessage : Message
    {
        public double Time;
    }
}