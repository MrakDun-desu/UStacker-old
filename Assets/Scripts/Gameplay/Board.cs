using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class Board : MonoBehaviour
    {
        public readonly List<Block[]> _blocks = new();
        public uint Width { get; set; }
        public Vector3 Up => transform.up;
        public Vector3 Right => transform.right;
        private Vector3 BoardDirection => transform.up + transform.right;
        
        private void ClearLine(int lineNumber)
        {
            if (_blocks.Count >= lineNumber) return;
            foreach (var block in _blocks[lineNumber]) {
                if (block == null) continue;
                block.Clear();
            }
            _blocks.RemoveAt(lineNumber);
            for (var i = lineNumber; i < _blocks.Count; i++) {
                foreach (var block in _blocks[i]) {
                    if (block == null) continue;
                    block.transform.position -= Up;
                }
            }
        }

        private void CheckAndClearLines()
        {
            for (var i = 0; i < _blocks.Count; i++) {
                var line = _blocks[i];
                var isFull = line.All(block => block != null);
                if (isFull) {
                    ClearLine(i);
                }
            }
        }

        public Vector2Int WorldSpaceToBoardPosition(Vector3 worldSpacePos)
        {
            worldSpacePos -= transform.position;
            var boardPosition = new Vector2(
                worldSpacePos.x / BoardDirection.x,
                worldSpacePos.y / BoardDirection.y
            );
            return new Vector2Int(Mathf.RoundToInt(boardPosition.x), Mathf.RoundToInt(boardPosition.y));
        }

        public Vector3 BoardPositionToWorldSpace(Vector2Int boardPos)
        {
            return new Vector3(
                boardPos.x * BoardDirection.x,
                boardPos.y * BoardDirection.y
            );
        }

        public bool CanPlace(Vector2Int blockPos)
        {
            if (blockPos.x < 0 || blockPos.x > Width ||
                blockPos.y < 0) return false;

            if (_blocks.Count <= blockPos.y) {
                return true;
            }

            return _blocks[blockPos.y][blockPos.x] == null;
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
            while (_blocks.Count <= blockPos.y) {
                _blocks.Add(new Block[Width]);
            }
            _blocks[blockPos.y][blockPos.x] = block;
            while (_blocks.Count <= blockPos.y) {
                _blocks.Add(new Block[Width]);
            }
            _blocks[blockPos.y][blockPos.x] = block;
        }

        public void Place(Piece piece)
        {
            if (!CanPlace(piece)) return;
            foreach (var block in piece.Blocks) {
                Place(block);
            }
            CheckAndClearLines();
        }

    }
}