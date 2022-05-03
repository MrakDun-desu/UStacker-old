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
        
        private Transform _helperTransform;
        
        private void ClearLine(int lineNumber)
        {
            if (Blocks.Count <= lineNumber) return;
            foreach (var block in Blocks[lineNumber]) {
                if (block == null) continue;
                block.Clear();
            }
            Blocks.RemoveAt(lineNumber);
            for (var i = lineNumber; i < Blocks.Count; i++) {
                foreach (var block in Blocks[i]) {
                    if (block == null) continue;
                    block.transform.position -= Up;
                }
            }
        }

        private int CheckAndClearLines()
        {
            var linesCleared = 0;
            for (var i = 0; i < Blocks.Count; i++) {
                var line = Blocks[i];
                var isFull = line.All(block => block != null);
                if (!isFull) continue;
                linesCleared++;
                ClearLine(i);
            }

            return linesCleared;
        }

        public Vector2Int WorldSpaceToBoardPosition(Vector3 worldSpacePos)
        {
            if (_helperTransform == null)
            {
                _helperTransform = new GameObject("Helper").transform;
                _helperTransform.SetParent(transform);
            }

            _helperTransform.position = worldSpacePos;
            return new Vector2Int((int)_helperTransform.localPosition.x,
                (int)_helperTransform.localPosition.y);
        }

        public Vector3 BoardPositionToWorldSpace(Vector2Int boardPos)
        {
            return transform.position + boardPos.x * Right + boardPos.y * Up;
        }

        public bool CanPlace(Vector2Int blockPos)
        {
            if (blockPos.x < 0 || blockPos.x >= Width ||
                blockPos.y < 0) return false;

            if (Blocks.Count <= blockPos.y) {
                return true;
            }

            return Blocks[blockPos.y][blockPos.x] == null;
        }

        public bool CanPlace(Block block, Vector2Int offset = new())
        {
            return CanPlace(WorldSpaceToBoardPosition(block.transform.position) + offset);
        }

        public bool CanPlace(Piece piece, Vector2Int offset = new())
        {
            return piece.Blocks.All(block => CanPlace(block, offset));
        }

        public void Place(Block block)
        {
            var blockPos = WorldSpaceToBoardPosition(block.transform.position);
            if (!CanPlace(blockPos)) return;
            while (Blocks.Count <= blockPos.y) {
                Blocks.Add(new Block[Width]);
            }
            Blocks[blockPos.y][blockPos.x] = block;
        }

        public bool Place(Piece piece)
        {
            if (!CanPlace(piece)) return false;
            foreach (var block in piece.Blocks) {
                Place(block);
            }
            var linesCleared = CheckAndClearLines();
            return linesCleared > 0;
        }

    }
}