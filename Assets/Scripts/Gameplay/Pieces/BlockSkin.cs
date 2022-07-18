using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockSkin : MonoBehaviour
    {
        [SerializeField] private SkinRecord _skinRecord;
        private SpriteRenderer _renderer;
        
        public SkinRecord SkinRecord
        {
            get => _skinRecord;
            set {
                _skinRecord = value;
                RefreshSkin();
            }
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            RefreshSkin();
        }

        private void RefreshSkin()
        {
            if (!_skinRecord.IsConnected)
            {
                if (_skinRecord.Sprites.Count > 0)
                {
                    var newSprite = _skinRecord.Sprites[0].Sprite;
                    if (newSprite is not null)
                        _renderer.sprite = _skinRecord.Sprites[0].Sprite;
                }

            }
            _renderer.sortingOrder = (int) _skinRecord.Layer;
        }

    }
}