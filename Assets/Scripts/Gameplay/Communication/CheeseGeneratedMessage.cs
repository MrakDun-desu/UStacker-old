using System.Collections.Generic;

namespace Blockstacker.Gameplay.Communication
{
    public class CheeseGeneratedMessage : Message
    {
        public List<bool[]> CheeseRows;
    }
}