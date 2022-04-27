using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Enums;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class Piece : MonoBehaviour
    {
        public PieceType PieceType;
        public List<Block> Blocks;
        public Vector2 SpawnOffset;
        public Vector2 ContainerOffset;
        public RotationState RotationState;

        public static event Action<Piece> PieceCleared;

        private void Awake()
        {
            foreach (var block in Blocks) {
                block.Cleared += OnBlockCleared;
                block.PieceType = PieceType;
            }
        }

        private void OnBlockCleared(Block sender)
        {
            Blocks.Remove(sender);
            if (Blocks.Count == 0)
            {
                PieceCleared?.Invoke(this);
            }
        }

        // public void RefreshBlocks()
        // {
        //     var blocksInChildren = GetComponentsInChildren<Block>();
        //     Blocks.AddRange(blocksInChildren);
        //     foreach (var block in Blocks) {
        //         block.Reset();
        //     }
        // }
    }
}