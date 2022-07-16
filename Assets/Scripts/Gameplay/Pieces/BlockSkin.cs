using System;
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
        }

    }
}