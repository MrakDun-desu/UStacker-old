using System;
using System.Linq;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine;

namespace Blockstacker.Gameplay.Blocks
{
    public class BlockBase : MonoBehaviour
    {
        [SerializeField] private uint _blockNumber;
        [SerializeField] private GameObject _blockSkinPrefab;
        [SerializeField] private GameObject _skinsParent;

        private string _collectionType;
        private IBlockCollection _blockCollection;

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
        }

        private void Start()
        {
            CollectionType = _blockCollection.Type;
            SkinLoader.SkinChanged += UpdateBlockSkin;
        }

        private void OnDestroy()
        {
            SkinLoader.SkinChanged -= UpdateBlockSkin;
        }

        private void UpdateBlockSkin()
        {
            var blockSkins = SkinLoader.SkinRecords
                .Where(record => record.PieceType == CollectionType && record.BlockNumbers.Contains(_blockNumber))
                .ToArray();

            if (blockSkins.Length == 0)
            {
                var defaultSkins = GetComponentsInChildren<BlockSkin>();
                foreach (var skin in defaultSkins)
                {
                    skin.Board = Board;
                    skin.BlockCollection = _blockCollection;
                    skin.RefreshSkin();
                }
                return;
            }

            foreach (Transform blockSkin in _skinsParent.transform)
            {
                Destroy(blockSkin.gameObject);
            }

            foreach (var skinRecord in blockSkins)
            {
                var newSkin = Instantiate(_blockSkinPrefab, _skinsParent.transform)
                    .GetComponent<BlockSkin>();
                newSkin.Board = Board;
                newSkin.BlockCollection = _blockCollection;
                newSkin.SkinRecord = skinRecord;
            }
        }
    }
}