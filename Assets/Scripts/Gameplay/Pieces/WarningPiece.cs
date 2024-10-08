
/************************************
WarningPiece.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UStacker.Common.Extensions;
using UStacker.Gameplay.Blocks;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;

namespace UStacker.Gameplay.Pieces
{
    public class WarningPiece : MonoBehaviour, IBlockCollection, IGameSettingsDependency
    {
        [SerializeField] private BlockBase _blockPrefab;
        [SerializeField] private Board _board;
        [SerializeField] private SpriteRenderer _warningLine;

        private readonly List<BlockBase> _blocks = new();
        private bool _awake;
        private ObjectPool<BlockBase> _blockPool;
        private string _currentPieceType = "";
        private bool _isEnabled = true;
        private GameSettingsSO.SettingsContainer _settings;

        private float _visibility;

        private float Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                foreach (var block in _blocks)
                    block.Visibility = _visibility;
            }
        }


        private void Awake()
        {
            if (_awake)
                return;

            _awake = true;
            _blockPool = new ObjectPool<BlockBase>(
                CreateBlock,
                block => block.Visibility = Visibility,
                block => block.Visibility = 0,
                block => Destroy(block.gameObject),
                true,
                4,
                20
            );
        }

        private void OnDestroy()
        {
            _blockPool.Dispose();
        }

        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);
        public string Type => "warning";

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set
            {
                _settings = value;
                Awake();
                Initialize();
            }
        }

        public event Action PieceChanged;

        private void Initialize()
        {
            if (_settings.General.NextPieceCount <= 0)
                _isEnabled = false;

            MakeInvisible();

            var warningLineTransform = _warningLine.transform;
            warningLineTransform.localScale =
                new Vector2(_settings.BoardDimensions.BoardWidth, warningLineTransform.localScale.y);
            warningLineTransform.localPosition = new Vector3(_settings.BoardDimensions.BoardWidth / 2f,
                _settings.BoardDimensions.BoardHeight);
        }

        public void MakeVisible()
        {
            if (!_isEnabled) return;

            Visibility = 1;

            if (_settings.Gravity.TopoutCondition == TopoutCondition.PieceSpawn) return;
            _warningLine.color = _warningLine.color.WithAlpha(1);
        }

        public void MakeInvisible()
        {
            Visibility = 0;

            _warningLine.color = _warningLine.color.WithAlpha(0);
        }

        private BlockBase CreateBlock()
        {
            var newBlock = Instantiate(_blockPrefab, transform);
            newBlock.Board = _board;
            newBlock.BlockNumber = (uint) Mathf.Min(_blocks.Count, 3);
            newBlock.Visibility = Visibility;
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
            Visibility = Visibility;
        }

        private void UpdateBlockCount(int blocksCount)
        {
            while (_blocks.Count > blocksCount)
            {
                _blockPool.Release(_blocks[^1]);
                _blocks.RemoveAt(_blocks.Count - 1);
            }

            while (_blocks.Count < blocksCount) _blocks.Add(_blockPool.Get());
        }
    }
}
/************************************
end WarningPiece.cs
*************************************/
