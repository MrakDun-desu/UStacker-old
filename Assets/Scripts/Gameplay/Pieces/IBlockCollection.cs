using System.Collections.Generic;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public interface IBlockCollection
    {
        IEnumerable<Vector3> BlockPositions { get; }
        string Type { get; }
    }
}