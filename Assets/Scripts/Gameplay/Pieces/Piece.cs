using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.GameSettings.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Pieces
{
    public class Piece : MonoBehaviour, IBlockCollection
    {
        [SerializeField] private string _type;
        public List<Block> Blocks = new();
        public Color GhostPieceColor;
        public Vector2 SpawnOffset;
        public Vector2 ContainerOffset;
        public RotationState RotationState;
        public uint MinimumSpinDetectors = 3;
        public Transform[] SpinDetectors = Array.Empty<Transform>();
        public Transform[] FullSpinDetectors = Array.Empty<Transform>();
        public UnityEvent PieceCleared;

        public event Action Rotated;

        private string _currentType;
        public string Type
        {
            get => _currentType;
            set
            {
                if (string.Equals(_currentType, value))
                    return;
                
                _currentType = value;
                foreach (var block in Blocks)
                    block.CollectionType = _currentType;
            }
        }

        public IEnumerable<Vector3> BlockPositions => 
            Blocks.Select(block => block.transform.position);

        private void Awake()
        {
            for (var i = 0; i < Blocks.Count; i++)
            {
                var block = Blocks[i];
                block.Cleared += OnBlockCleared;
                block.BlockNumber = (uint)i;
            }
        }

        private void Start()
        {
            _currentType = _type;
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

        public void RevertType() => Type = _type;

    }
}