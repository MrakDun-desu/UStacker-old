using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class Piece : MonoBehaviour
    {
        public PieceType PieceType;
        public List<Block> Blocks;

        public event Action BlockCleared;

        private void Awake()
        {
            RefreshBlocks();
            foreach (var block in Blocks) {
                block.Cleared += OnBlockCleared;
                block.PieceType = PieceType;
            }
        }

        private void OnBlockCleared(Block sender)
        {
            Blocks.Remove(sender);
            if (Blocks.Count == 0) {
                enabled = false;
                return;
            }
            BlockCleared?.Invoke();
        }

        public void RefreshBlocks()
        {
            var blocksInChildren = GetComponentsInChildren<Block>();
            Blocks.AddRange(blocksInChildren);
            foreach (var block in Blocks) {
                block.Reset();
            }
        }
    }
}