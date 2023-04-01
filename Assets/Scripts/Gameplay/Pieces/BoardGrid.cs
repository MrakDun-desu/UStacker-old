using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Blocks;
using UnityEngine;
using UnityEngine.Pool;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;
using UStacker.GlobalSettings;

namespace UStacker.Gameplay.Pieces
{
    public class BoardGrid : MonoBehaviour, IBlockCollection, IGameSettingsDependency
    {
        [SerializeField] private Board _board;
        [SerializeField] private BlockBase _blockPrefab;
        
        private readonly List<BlockBase> _blocks = new();
        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);
        public string Type => "grid";

        private ObjectPool<BlockBase> _blockPool;

        private GameSettingsSO.SettingsContainer _gameSettings;
        private bool _awake;

        public GameSettingsSO.SettingsContainer GameSettings
        {
            private get => _gameSettings;
            set
            {
                _gameSettings = value;
                Awake();
                Initialize();
            }
        }

        private void Awake()
        {
            if (_awake)
                return;

            _awake = true;
            _blockPool = new ObjectPool<BlockBase>(
                () => Instantiate(_blockPrefab, transform),
                block => block.Visibility = AppSettings.Gameplay.GridVisibility,
                block => block.Visibility = 0,
                block => Destroy(block.gameObject),
                true, 200);
        }

        private void OnDestroy()
        {
            _blockPool.Dispose();
        }

        private void Initialize()
        {
            foreach (var block in _blocks)
                _blockPool.Release(block);
            
            _blocks.Clear();
            
            for (var y = 0; y < GameSettings.BoardDimensions.BoardHeight; y++)
            for (var x = 0; x < GameSettings.BoardDimensions.BoardWidth; x++)
            {
                var gridBlock = _blockPool.Get();

                var blockTransform = gridBlock.transform;
                blockTransform.localPosition = new Vector3(x + .5f, y + .5f, blockTransform.localPosition.z);

                _blocks.Add(gridBlock);
                gridBlock.Board = _board;
            }
        }
    }
}