using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Enums;
using Blockstacker.GameSettings.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Pieces
{
    public class Piece : MonoBehaviour
    {
        public PieceType PieceType;
        public List<Block> Blocks;
        public Vector2 SpawnOffset;
        public Vector2 ContainerOffset;
        public RotationState RotationState;
        public UnityEvent PieceCleared;

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
            if (Blocks.Count != 0) return;
            PieceCleared.Invoke();
            Destroy(gameObject);
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