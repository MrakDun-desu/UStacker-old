using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class BoardGrid : MonoBehaviour, IBlockCollection
    {
        [SerializeField] private Board _board;
        private List<BlockBase> _blocks = new();
        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);
        public string Type => "grid";

        public void AddBlock(BlockBase block)
        {
            _blocks.Add(block);
            block.Board = _board;
        }
    }
}