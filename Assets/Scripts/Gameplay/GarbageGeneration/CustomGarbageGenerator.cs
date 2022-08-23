using System;
using Blockstacker.Gameplay.Communication;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class CustomGarbageGenerator : IGarbageGenerator
    {
        public CustomGarbageGenerator(ReadonlyBoard board, string script, out string validationErrors)
        {
            throw new NotImplementedException();
        }

        public void ResetState(int seed)
        {
            throw new NotImplementedException();
        }

        public void GenerateGarbage(uint amount, PiecePlacedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}