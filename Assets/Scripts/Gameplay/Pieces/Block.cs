using System;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Pieces
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onCleared;
        [SerializeField] private Vector2 _initialPosition;

        public void Reset()
        {
            transform.localPosition = new Vector3(
                _initialPosition.x,
                _initialPosition.y,
                transform.localPosition.z
            );
        }

        private void OnValidate()
        {
            Reset();
        }

        public event Action<Block> Cleared;

        public void Clear()
        {
            _onCleared.Invoke();
            Cleared?.Invoke(this);
        }
    }
}