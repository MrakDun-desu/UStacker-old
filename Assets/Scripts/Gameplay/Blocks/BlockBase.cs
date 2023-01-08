using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Pieces;
using UStacker.GlobalSettings.BlockSkins;
using UnityEngine;
using UnityEngine.Pool;

namespace UStacker.Gameplay.Blocks
{
    public class BlockBase : MonoBehaviour
    {
        [SerializeField] private BlockSkin _blockSkinPrefab;
        [SerializeField] protected GameObject _skinsParent;
        private readonly List<BlockSkin> _currentSkins = new();
        private IBlockCollection _blockCollection;

        private string _collectionType;
        private BlockSkin[] _defaultSkins;
        private ObjectPool<BlockSkin> _skinsPool;
        private float _visibility = 1;
        private bool _usingDefault = true;

        [field: SerializeField]
        public uint BlockNumber { get; set; }
        public float Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                foreach (var skin in _currentSkins) skin.Visibility = _visibility;
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
            _skinsPool = new ObjectPool<BlockSkin>(
                () => Instantiate(_blockSkinPrefab),
                skin => skin.gameObject.SetActive(true),
                OnSkinRelease,
                skin => Destroy(skin.gameObject));
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

        private void OnSkinRelease(BlockSkin skin)
        {
            skin.gameObject.SetActive(false);
            skin.UnregisterEvents();
        }

        protected virtual void UpdateBlockSkin()
        {
            if (!TryGetSkins(out var newSkins))
            {
                if (!_usingDefault)
                {
                    foreach (var skin in _currentSkins)
                        _skinsPool.Release(skin);
                    _currentSkins.Clear();
                }

                foreach (var skin in _defaultSkins)
                {
                    skin.Board = Board;
                    skin.BlockCollection = _blockCollection;
                    skin.RefreshSkin();
                    _currentSkins.Add(skin);
                }

                _usingDefault = true;
                return;
            }

            _usingDefault = false;
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
                    .Where(record => record.SkinType == simplifiedCollectionType && record.BlockNumbers.Contains(BlockNumber))
                    .ToArray();
            }

            return newSkins.Length != 0;
        }

        protected void ReplaceOldSkins(IEnumerable<SkinRecord> newSkins, GameObject parent, bool forceDestroyOld = false)
        {
            var skinsExisted = _currentSkins.Count <= 0;
            foreach (var skin in _currentSkins)
                _skinsPool.Release(skin);
            _currentSkins.Clear();
            
            if (skinsExisted || forceDestroyOld)
                foreach (Transform tf in parent.transform) Destroy(tf.gameObject);

            AddNewSkins(newSkins, parent);
        }

        private void AddNewSkins(IEnumerable<SkinRecord> newSkins, GameObject parent)
        {
            foreach (var skinRecord in newSkins)
            {
                var newSkin = _skinsPool.Get();
                newSkin.Board = Board;
                newSkin.BlockCollection = _blockCollection;
                newSkin.transform.SetParent(parent.transform, false);
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