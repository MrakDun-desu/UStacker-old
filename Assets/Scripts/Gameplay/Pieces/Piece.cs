using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.GameSettings.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

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

        private string _currentType;
        private readonly List<Transform> _activeTransforms = new();
        public event Action Rotated;
        
        public ObjectPool<Piece> SourcePool { get; set; }
        
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
            _activeTransforms.Select(tf => tf.position);

        private void Awake()
        {
            _currentType = _type;
            _activeTransforms.AddRange(Blocks.Select(block => block.transform));
            for (var i = 0; i < Blocks.Count; i++)
            {
                var block = Blocks[i];
                block.Cleared += OnBlockCleared;
                block.BlockNumber = (uint)i;
            }
        }

        private void OnBlockCleared(Block sender)
        {
            _activeTransforms.Remove(sender.transform);
            if (Blocks.Count != 0) return;
            PieceCleared.Invoke();
            SourcePool.Release(this);
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

        public void ResetState()
        {
            gameObject.SetActive(true);
            foreach (var block in Blocks)
            {
                if (!_activeTransforms.Contains(block.transform))
                    _activeTransforms.Add(block.transform);
                    
                block.gameObject.SetActive(true);
                block.ResetPosition();
            }
        }

        [ContextMenu("Log block positions")]
        private void LogBlockPositions()
        {
            foreach (var block in Blocks.Where(block => _activeTransforms.Contains(block.transform)))
            {
                Debug.Log(block.Board.WorldSpaceToBoardPosition(block.transform.position));
            }
        }
    }
}