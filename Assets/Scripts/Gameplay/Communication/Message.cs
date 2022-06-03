using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public abstract record Message
    {
        public double Time;
    }
}