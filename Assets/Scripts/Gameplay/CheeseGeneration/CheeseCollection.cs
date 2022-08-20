using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.Pieces;
using UnityEngine;

namespace Blockstacker.Gameplay.CheeseGeneration
{
    public class CheeseCollection : MonoBehaviour, IBlockCollection
    {
        private List<BlockBase> _blocks;
        public string Type => "cheese";
        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);

        public void AddBlock(BlockBase block)
        {
            _blocks.Add(block);
        }

    }
}