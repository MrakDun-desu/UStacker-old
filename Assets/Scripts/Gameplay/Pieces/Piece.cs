using System;
using System.Collections.Generic;
using Blockstacker.GameSettings.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Pieces
{
    public class Piece : MonoBehaviour
    {
        public List<Block> Blocks = new();
        public string PieceType;
        public Color GhostPieceColor;
        public Vector2 SpawnOffset;
        public Vector2 ContainerOffset;
        public RotationState RotationState;
        public uint MinimumSpinDetectors = 3;
        public Transform[] SpinDetectors = Array.Empty<Transform>();
        public Transform[] FullSpinDetectors = Array.Empty<Transform>();
        public UnityEvent PieceCleared;

        public event Action Rotated;

        private void Start()
        {
            foreach (var block in Blocks)
            {
                block.Cleared += OnBlockCleared;
            }
        }

        private void OnBlockCleared(Block sender)
        {
            Blocks.Remove(sender);
            if (Blocks.Count != 0) return;
            PieceCleared.Invoke();
            Destroy(gameObject);
        }

        public void Rotate(int rotationAngle)
        {
            transform.Rotate(Vector3.forward, rotationAngle);
            Rotated?.Invoke();
        }

        public void SetBoard(Board board)
        {
            foreach (var block in Blocks)
            {
                block.Board = board;
            }
        }
        
    }
}