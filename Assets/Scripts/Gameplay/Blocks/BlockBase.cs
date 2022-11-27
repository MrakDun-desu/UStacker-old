using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine;

namespace Blockstacker.Gameplay.Blocks
{
    public class BlockBase : MonoBehaviour
    {
        [SerializeField] private BlockSkin _blockSkinPrefab;
        [SerializeField] protected GameObject _skinsParent;

        private string _collectionType;
        private IBlockCollection _blockCollection;
        private BlockSkin[] _defaultSkins;
        private readonly List<BlockSkin> _currentSkins = new();
        private float _visibility = 1;

        public uint BlockNumber { get; set; }
        public float Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                foreach (var skin in _currentSkins)
                {
                    skin.Visibility = _visibility;
                }
            }
        }
        public string CollectionType
        {
            get => _collectionType;
            set
            {
                _collectionType = value;
                UpdateBlockSkin();
            }
        }
        public Board Board { get; set; }

        private void Awake()
        {
            _blockCollection = GetComponentInParent<IBlockCollection>();
            _defaultSkins = _skinsParent.GetComponentsInChildren<BlockSkin>();
        }

        protected virtual void Start()
        {
            CollectionType = _blockCollection.Type;
            SkinLoader.SkinChanged += UpdateBlockSkin;
        }

        private void OnDestroy()
        {
            SkinLoader.SkinChanged -= UpdateBlockSkin;
        }

        protected virtual void UpdateBlockSkin()
        {
            _currentSkins.Clear();
            if (!TryGetSkins(out var newSkins))
            {
                foreach (var skin in _defaultSkins)
                {
                    skin.Board = Board;
                    skin.BlockCollection = _blockCollection;
                    skin.RefreshSkin();
                    _currentSkins.Add(skin);
                }
                return;
            }

            ReplaceOldSkins(newSkins, _skinsParent);
        }

        protected bool TryGetSkins(out SkinRecord[] newSkins)
        {
            newSkins = SkinLoader.SkinRecords
                .Where(record => record.SkinType == CollectionType && record.BlockNumbers.Contains(BlockNumber))
                .ToArray();

            if (newSkins.Length == 0 && CollectionType.StartsWith("giant"))
            {
                var simplifiedCollectionType = CollectionType[^1].ToString().ToLowerInvariant();
                newSkins = SkinLoader.SkinRecords
                    .Where(record => record.SkinType == simplifiedCollectionType)
                    .ToArray();
            }

            return newSkins.Length != 0;
        }

        protected void ReplaceOldSkins(IEnumerable<SkinRecord> newSkins, GameObject parent)
        {
            foreach (Transform blockSkin in parent.transform)
                Destroy(blockSkin.gameObject);
            
            AddNewSkins(newSkins, parent);
        }

        private void AddNewSkins(IEnumerable<SkinRecord> newSkins, GameObject parent)
        {
            foreach (var skinRecord in newSkins)
            {
                var newSkin = Instantiate(_blockSkinPrefab, parent.transform);
                newSkin.Board = Board;
                newSkin.BlockCollection = _blockCollection;
                newSkin.SkinRecord = skinRecord;
                _currentSkins.Add(newSkin);
            }
        }

        public void SetBlockCollection(IBlockCollection newCollection)
        {
            _blockCollection = newCollection;
        }

        public void RefreshSkins()
        {
            foreach (var skin in _currentSkins)
            {
                skin.UnregisterEvents();
                skin.BlockCollection = _blockCollection;
                skin.RefreshSkin();
            }
        }

    }
}