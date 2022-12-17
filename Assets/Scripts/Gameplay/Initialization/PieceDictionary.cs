using System;
using UStacker.Common;
using UStacker.Gameplay.Pieces;

namespace UStacker.Gameplay.Initialization
{
    [Serializable]
    public class PieceDictionary : SerializableDictionary<string, Piece>
    {
    }
}