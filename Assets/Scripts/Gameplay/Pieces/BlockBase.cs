using System.Linq;
using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class BlockBase : MonoBehaviour
    {
        [SerializeField] private uint _blockNumber;
        [SerializeField] private GameObject _blockSkinPrefab;
        [SerializeField] private GameObject _skinsParent;

        private string _collectionType;
        private IBlockCollection _blockCollection;

        public Board Board { get; set; }

        private void Awake()
        {
            _blockCollection = GetComponentInParent<IBlockCollection>();
        }

        private void Start()
        {
            _collectionType = _blockCollection.Type;
            UpdateBlockSkin();
            SkinLoader.SkinChanged += UpdateBlockSkin;
        }

        private void UpdateBlockSkin()
        {
            var skinRecords = SkinLoader.SkinRecords;
            var blockSkins =
                skinRecords.Where(
                    record => record.PieceType == _collectionType && record.BlockNumbers.Contains(_blockNumber)).ToArray();

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