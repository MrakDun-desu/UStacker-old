using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.Initialization;
using Blockstacker.GameSettings;
using UnityEngine;
using UnityEngine.Pool;

namespace Blockstacker.Gameplay.Pieces
{
    public class WarningPiece : MonoBehaviour, IBlockCollection, IGameSettingsDependency
    {
        [SerializeField] private BlockBase _blockPrefab;
        [SerializeField] private Board _board;

        public GameSettingsSO GameSettings { set => _settings = value; }
        private GameSettingsSO _settings;

        private readonly List<BlockBase> _blocks = new();
        private string _currentPieceType = "";
        private bool _isEnabled = true;
        private ObjectPool<BlockBase> _blockPool;

        public event Action PieceChanged;

        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);
        public string Type => "warning";

        public void MakeVisible()
        {
            if (_isEnabled)
                gameObject.SetActive(true);
        }

        public void MakeInvisible() 
        {
            if (_isEnabled)
                gameObject.SetActive(false);
        }

        private void Awake()
        {
            _blockPool = new ObjectPool<BlockBase>(
                CreateBlock,
                block => block.gameObject.SetActive(true),
                block => block.gameObject.SetActive(false),
                block => Destroy(block.gameObject),
                true,
                4,
                20
            );
        }

        private void Start()
        {
            if (_settings.General.NextPieceCount <= 0) 
                _isEnabled = false;
            
            gameObject.SetActive(false);
        }
        
        private BlockBase CreateBlock()
        {
            var newBlock = Instantiate(_blockPrefab, transform);
            newBlock.Board = _board;
            newBlock.BlockNumber = (uint)Mathf.Min(_blocks.Count, 3);
            return newBlock;
        }

        public void SetPiece(Piece piece)
        {
            if (!enabled || !_isEnabled) return;
            if (piece.Type == _currentPieceType)
                return;

            _currentPieceType = piece.Type;

            UpdateBlockCount(piece.Blocks.Count);

            var boardTransform = _board.transform;
            var piecePos = new Vector3(
                (int) (_settings.BoardDimensions.BoardWidth / 2u),
                (int) _settings.BoardDimensions.PieceSpawnHeight,
                boardTransform.position.z
            );
            transform.localPosition = piecePos + new Vector3(piece.SpawnOffset.x, piece.SpawnOffset.y);

            for (var i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].transform.localPosition = piece.Blocks[i].transform.localPosition;
                _blocks[i].transform.rotation = piece.Blocks[i].transform.rotation;
            }

            var rotationSystem = _settings.Controls.ActiveRotationSystem;
            var rotation = rotationSystem.GetKickTable(_currentPieceType).StartState;
            transform.Rotate(Vector3.forward, (float) rotation);

            PieceChanged?.Invoke();
        }

        private void UpdateBlockCount(int blocksCount)
        {
            while (_blocks.Count > blocksCount)
            {
                _blockPool.Release(_blocks[^1]);
                _blocks.RemoveAt(_blocks.Count - 1);
            }

            while (_blocks.Count < blocksCount)
            {
                _blocks.Add(_blockPool.Get());
            }
        }
    }
}