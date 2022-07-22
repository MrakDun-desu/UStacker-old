using System;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Blocks
{
    public class Block : BlockBase
    {
        [SerializeField] private UnityEvent _onCleared;
        [SerializeField] private Vector2 _initialPosition;

        public event Action<Block> Cleared;

        private void OnValidate()
        {
            ResetPosition();
        }

        private void ResetPosition()
        {
            var myTransform = transform;
            myTransform.localPosition = new Vector3(
                _initialPosition.x,
                _initialPosition.y,
                myTransform.localPosition.z
            );
        }

        public void Clear()
        {
            _onCleared.Invoke();
            Cleared?.Invoke(this);
        }
    }

}