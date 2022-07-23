using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine;

namespace Blockstacker.Gameplay.Blocks
{
    public class BlockBase : MonoBehaviour
    {
        public uint BlockNumber;
        [SerializeField] private BlockSkin _blockSkinPrefab;
        [SerializeField] protected GameObject _skinsParent;

        private string _collectionType;
        private IBlockCollection _blockCollection;
        private BlockSkin[] _defaultSkins;

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
            _defaultSkins = GetComponentsInChildren<BlockSkin>();
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

        private void AddNewSkins(IEnumerable<SkinRecord> newSkins)
        {
            foreach (var skinRecord in newSkins)
            {
                var newSkin = Instantiate(_blockSkinPrefab, _skinsParent.transform);
                newSkin.Board = Board;
                newSkin.BlockCollection = _blockCollection;
                newSkin.SkinRecord = skinRecord;
            }
        }

        protected virtual void UpdateBlockSkin()
        {
            if (!TryGetSkins(out var newSkins))
            {
                foreach (var skin in _defaultSkins)
                {
                    skin.Board = Board;
                    skin.BlockCollection = _blockCollection;
                    skin.RefreshSkin();
                }
                return;
            }

            ReplaceOldSkins(newSkins);
        }

        protected bool TryGetSkins(out SkinRecord[] newSkins)
        {
            newSkins = SkinLoader.SkinRecords
                .Where(record => record.PieceType == CollectionType && record.BlockNumbers.Contains(BlockNumber))
                .ToArray();

            return newSkins.Length != 0;
        }

        protected void ReplaceOldSkins(IEnumerable<SkinRecord> newSkins)
        {
            foreach (Transform blockSkin in _skinsParent.transform)
                Destroy(blockSkin.gameObject);
            
            AddNewSkins(newSkins);
        }

    }
}