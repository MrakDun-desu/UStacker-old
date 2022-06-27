using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record CheeseGeneratedMessage : MidgameMessage
    {
        public List<bool[]> CheeseRows;
    }
}