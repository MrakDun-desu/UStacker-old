using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record CheeseGeneratedMessage : Message
    {
        public List<bool[]> CheeseRows;
    }
}