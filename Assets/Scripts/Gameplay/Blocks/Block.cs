using System;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Blocks
{
    public class Block : BlockBase
    {
        [SerializeField] private UnityEvent _onCleared;
        [SerializeField] private Vector2 _initialPosition;
        [SerializeField] private GameObject _holdSkinsParent;

        public event Action<Block> Cleared;

        private string _originalCollectionType;

        private void OnValidate()
        {
            ResetPosition();
        }

        protected override void Start()
        {
            base.Start();
            _originalCollectionType = CollectionType;
        }

        public void ResetPosition()
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

        protected override void UpdateBlockSkin()
        {
            if (CollectionType == PieceContainer.USED_HOLD_TYPE)
                ChangeSkin(true);
            else if (CollectionType == _originalCollectionType)
                ChangeSkin(false);
            else
            {
                base.UpdateBlockSkin();
                _holdSkinsParent.gameObject.SetActive(false);
                _skinsParent.SetActive(true);
            }
        }

        private void ChangeSkin(bool newIsHold)
        {
            if (TryGetSkins(out var newSkins))
            {
                ReplaceOldSkins(newSkins, newIsHold ? _holdSkinsParent : _skinsParent);
            }

            _skinsParent.SetActive(!newIsHold);
            _holdSkinsParent.gameObject.SetActive(newIsHold);
        }
    }
}