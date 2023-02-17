using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Blocks;
using UStacker.GameSettings.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace UStacker.Gameplay.Pieces
{
    public class Piece : MonoBehaviour, IBlockCollection
    {
        [SerializeField] private string _type;
        public List<PieceBlock> Blocks = new();
        public Color GhostPieceColor;
        public Vector2 SpawnOffset;
        public Vector2 ContainerOffset;
        public RotationState RotationState;
        public uint MinimumSpinDetectors = 3;
        public Transform[] SpinDetectors = Array.Empty<Transform>();
        public Transform[] FullSpinDetectors = Array.Empty<Transform>();
        public UnityEvent PieceCleared;
        private readonly List<Transform> _activeTransforms = new();

        private bool _activeInPool = true;
        private string _currentType;

        private float _visibility;

        public float Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                foreach (var block in Blocks) block.Visibility = _visibility;
            }
        }

        public ObjectPool<Piece> SourcePool { get; set; }

        private void Awake()
        {
            _currentType = _type;
            _activeTransforms.AddRange(Blocks.Select(block => block.transform));
            for (var i = 0; i < Blocks.Count; i++)
            {
                var block = Blocks[i];
                block.Cleared += OnBlockCleared;
                block.BlockNumber = (uint) Mathf.Min(i, 3);
            }
        }

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

        public event Action Rotated;

        private void OnBlockCleared(ClearableBlock sender)
        {
            _activeTransforms.Remove(sender.transform);
            if (_activeTransforms.Count != 0) return;
            PieceCleared.Invoke();
            ReleaseFromPool();
        }

        public void Rotate(int rotationAngle)
        {
            transform.Rotate(Vector3.forward, rotationAngle);
            Rotated?.Invoke();
        }

        public void SetBoard(Board board)
        {
            foreach (var block in Blocks) block.Board = board;
        }

        public void RevertType()
        {
            Type = _type;
        }

        public void Deactivate()
        {
            Visibility = 0;
        }

        public void Activate()
        {
            _activeInPool = true;
            Visibility = 1;
            foreach (var block in Blocks)
            {
                if (!_activeTransforms.Contains(block.transform))
                    _activeTransforms.Add(block.transform);

                block.ResetPosition();
            }

            RotationState = RotationState.Zero;
            Rotated?.Invoke();
        }

        [ContextMenu("Return to the pool")]
        public void ReleaseFromPool()
        {
            if (!_activeInPool) return;

            SourcePool.Release(this);
            _activeInPool = false;
        }

        [ContextMenu("Log block positions")]
        private void LogBlockPositions()
        {
            foreach (var block in Blocks.Where(block => _activeTransforms.Contains(block.transform)))
                Debug.Log(block.Board.WorldSpaceToBoardPosition(block.transform.position));
        }
    }
}