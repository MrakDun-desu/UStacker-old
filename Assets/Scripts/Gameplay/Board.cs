using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class Board : MonoBehaviour
    {
        public readonly List<Block[]> Blocks = new();
        public uint Width { get; set; }
        public uint Height { get; set; }
        public Vector3 Up => transform.up * transform.localScale.y;
        public Vector3 Right => transform.right * transform.localScale.x;

        [SerializeField] private Transform _helperTransform;

        private void ClearLine(int lineNumber)
        {
            if (Blocks.Count <= lineNumber) return;
            foreach (var block in Blocks[lineNumber])
            {
                if (block == null) continue;
                block.Clear();
            }

            Blocks.RemoveAt(lineNumber);
            for (var i = lineNumber; i < Blocks.Count; i++)
            {
                foreach (var block in Blocks[i])
                {
                    if (block == null) continue;
                    block.transform.position -= Up;
                }
            }
        }

        private int CheckAndClearLines()
        {
            var linesCleared = 0;
            for (var i = 0; i < Blocks.Count; i++)
            {
                var line = Blocks[i];
                var isFull = line.All(block => block != null);
                if (!isFull) continue;
                linesCleared++;
                ClearLine(i);
                i--;
            }

            return linesCleared;
        }

        public Vector2Int WorldSpaceToBoardPosition(Vector3 worldSpacePos)
        {
            _helperTransform.position = worldSpacePos;
            var localPosition = _helperTransform.localPosition;
            return new Vector2Int(Mathf.FloorToInt(localPosition.x),
                Mathf.FloorToInt(localPosition.y));
        }

        public Vector3 BoardPositionToWorldSpace(Vector2Int boardPos) =>
            transform.position + boardPos.x * Right + boardPos.y * Up;

        public bool CanPlace(Vector2Int blockPosition)
        {
            if (blockPosition.x < 0 || blockPosition.x >= Width ||
                blockPosition.y < 0) return false;

            if (Blocks.Count <= blockPosition.y)
            {
                return true;
            }

            return Blocks[blockPosition.y][blockPosition.x] is null;
        }

        public bool CanPlace(Vector3 realPosition, Vector2Int offset = new())
        {
            var boardPosition = WorldSpaceToBoardPosition(realPosition);
            return CanPlace(boardPosition + offset);
        }

        public bool CanPlace(Piece piece, Vector2Int offset = new())
        {
            return piece.Blocks.All(block => CanPlace(block.transform.position, offset));
        }

        public bool CanPlace(IEnumerable<Transform> transforms, Vector2Int offset = new())
        {
            return transforms.All(tf => CanPlace(tf.position, offset));
        }

        public void Place(Block block)
        {
            var blockPos = WorldSpaceToBoardPosition(block.transform.position);
            if (!CanPlace(blockPos)) return;
            while (Blocks.Count <= blockPos.y)
            {
                Blocks.Add(new Block[Width]);
            }

            Blocks[blockPos.y][blockPos.x] = block;
        }

        public bool Place(Piece piece)
        {
            if (!CanPlace(piece)) return false;
            foreach (var block in piece.Blocks)
            {
                Place(block);
            }

            var linesCleared = CheckAndClearLines();
            return linesCleared > 0;
        }
    }
}