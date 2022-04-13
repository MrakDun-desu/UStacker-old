using System.Collections.Generic;

namespace Blockstacker.Gameplay.Communication
{
    public class CheeseGeneratedMessage : IMessage
    {
        public List<bool[]> cheeseRows;
    }
}