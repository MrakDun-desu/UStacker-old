using System;
using Blockstacker.Gameplay.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Pieces
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private UnityAction _onCleared;
        [SerializeField] private Vector2 _initialPosition;
        public PieceType PieceType;
        public event Action<Block> Cleared;

        private void OnDisable()
        {
            Clear();
        }

        private void OnValidate()
        {
            Reset();
        }

        public void Clear()
        {
            _onCleared.Invoke();
            Cleared?.Invoke(this);
        }

        public void Reset()
        {
            transform.position = new Vector3(
                _initialPosition.x,
                _initialPosition.y,
                transform.position.z
            );
        }
    }
}