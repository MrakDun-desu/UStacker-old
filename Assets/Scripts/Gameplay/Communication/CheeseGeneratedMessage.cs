using System.Collections.Generic;

namespace Blockstacker.Gameplay.Communication
{
    public record CheeseGeneratedMessage : Message
    {
        public List<bool[]> CheeseRows;
    }
}