using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Blocks;
using UnityEngine;

namespace UStacker.Gameplay.Pieces
{
    public class BoardGrid : MonoBehaviour, IBlockCollection
    {
        [SerializeField] private Board _board;
        private readonly List<BlockBase> _blocks = new();
        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);
        public string Type => "grid";

        public void AddBlock(BlockBase block)
        {
            _blocks.Add(block);
            block.Board = _board;
        }
    }
}