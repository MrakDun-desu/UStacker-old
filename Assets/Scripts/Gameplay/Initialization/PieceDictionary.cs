using System;
using Blockstacker.Common;
using Blockstacker.Gameplay.Pieces;

namespace Blockstacker.Gameplay.Initialization
{
    [Serializable]
    public class PieceDictionary : SerializableDictionary<string, Piece>
    {
    }
}