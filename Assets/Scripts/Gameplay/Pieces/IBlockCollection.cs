
/************************************
IBlockCollection.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Gameplay.Pieces
{
    public interface IBlockCollection
    {
        IEnumerable<Vector3> BlockPositions { get; }
        string Type { get; }
    }
}
/************************************
end IBlockCollection.cs
*************************************/
