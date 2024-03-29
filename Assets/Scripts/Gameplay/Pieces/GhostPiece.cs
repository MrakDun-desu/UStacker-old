﻿using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Blocks;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.Appliers;
using UnityEngine;
using UnityEngine.Pool;

namespace UStacker.Gameplay.Pieces
{
    public class GhostPiece : MonoBehaviour, IBlockCollection, IGameSettingsDependency
    {

        private static readonly Color _defaultColor = Color.white;
        [SerializeField] private BlockBase _blockPrefab;
        [SerializeField] private Board _board;
        private readonly List<BlockBase> _blocks = new();
        private Piece _activePiece;
        private ObjectPool<BlockBase> _blockPool;
        private bool _colorGhostPiece;
        private Color _currentColor = _defaultColor;
        private GameSettingsSO.SettingsContainer _settings;

        public Piece ActivePiece
        {
            get => _activePiece;
            set
            {
                if (!_settings.Controls.ShowGhostPiece) return;

                _activePiece = value;
                CurrentColor = _activePiece.GhostPieceColor;

                UpdateBlockCount();
            }
        }

        public Color CurrentColor
        {
            get => _currentColor;
            private set
            {
                var newColor = _colorGhostPiece ? value : _defaultColor;
                if (_currentColor == newColor)
                    return;
                _currentColor = newColor;
                ColorChanged?.Invoke(_currentColor);
            }
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
            if (!_settings.Controls.ShowGhostPiece)
                gameObject.SetActive(false);

            _colorGhostPiece = AppSettings.Gameplay.ColorGhostPiece;
            ColorGhostPieceApplier.ColorGhostPieceChanged += ChangeColoring;
        }

        private void OnDestroy()
        {
            ColorGhostPieceApplier.ColorGhostPieceChanged -= ChangeColoring;
        }

        public IEnumerable<Vector3> BlockPositions =>
            _blocks.Select(block => block.transform.position);

        public string Type => "ghost";

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set => _settings = value;
        }
        public event Action<Color> ColorChanged;
        public event Action Rendered;

        private BlockBase CreateBlock()
        {
            var newBlock = Instantiate(_blockPrefab, transform);
            newBlock.Board = _board;
            newBlock.BlockNumber = (uint) Mathf.Min(_blocks.Count, 3);
            return newBlock;
        }

        private void ChangeColoring(bool colorGhostPiece)
        {
            _colorGhostPiece = colorGhostPiece;
            CurrentColor = colorGhostPiece ? _activePiece.GhostPieceColor : _defaultColor;
        }

        private void UpdateBlockCount()
        {
            while (_blocks.Count > ActivePiece.Blocks.Count)
            {
                _blockPool.Release(_blocks[^1]);
                _blocks.RemoveAt(_blocks.Count - 1);
            }

            while (_blocks.Count < ActivePiece.Blocks.Count) _blocks.Add(_blockPool.Get());
        }

        public void Render()
        {
            if (!_settings.Controls.ShowGhostPiece) return;

            transform.position = ActivePiece.transform.position;
            for (var i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].transform.position = ActivePiece.Blocks[i].transform.position;
                _blocks[i].transform.rotation = ActivePiece.Blocks[i].transform.rotation;
            }

            var moveVector = Vector2Int.down;
            while (_board.CanPlace(_blocks, moveVector)) moveVector += Vector2Int.down;
            moveVector -= Vector2Int.down;

            var pieceTransform = transform;
            var piecePosition = pieceTransform.localPosition;
            piecePosition = new Vector3(
                piecePosition.x + moveVector.x,
                piecePosition.y + moveVector.y,
                piecePosition.z);
            pieceTransform.localPosition = piecePosition;

            Rendered?.Invoke();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }
    }
}