using UnityEngine;

namespace Blockstacker.Gameplay.Blocks
{
    public class Block : ClearableBlock
    {
        [SerializeField] private Vector2 _initialPosition;
        [SerializeField] private GameObject _holdSkinsParent;
        private bool _firstTimeLoad = true;

        private string _originalCollectionType;

        protected override void Start()
        {
            base.Start();
            _originalCollectionType = CollectionType;
        }

        private void OnValidate()
        {
            ResetPosition();
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
            if (_firstTimeLoad)
            {
                if (TryGetSkins(out var newSkins))
                    ReplaceOldSkins(newSkins, newIsHold ? _holdSkinsParent : _skinsParent, newIsHold);
                if (newIsHold)
                    _firstTimeLoad = false;
            }

            _skinsParent.SetActive(!newIsHold);
            _holdSkinsParent.gameObject.SetActive(newIsHold);
        }

        [ContextMenu("Set initial position")]
        private void SetInitialPosition()
        {
            var localPosition = transform.localPosition;
            _initialPosition.x = localPosition.x;
            _initialPosition.y = localPosition.y;
        }
    }
}