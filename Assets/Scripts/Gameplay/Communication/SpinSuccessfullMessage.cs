using System;
using Blockstacker.Gameplay.Spins;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record SpinSuccessfullMessage : Message
    {
        public SpinResult SpinResult;
    }
}