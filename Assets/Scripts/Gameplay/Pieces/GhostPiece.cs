using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Appliers;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class GhostPiece : MonoBehaviour, IBlockCollection
    {
        [SerializeField] private List<BlockBase> Blocks = new();
        [SerializeField] private Board _board;
        [SerializeField] private GameSettingsSO _settings;

        private static readonly Color _defaultColor = Color.white;
        private Piece _activePiece;
        private Color _currentColor = _defaultColor;
        private bool _colorGhostPiece;

        public Piece ActivePiece
        {
            get => _activePiece;
            set
            {
                if (!_settings.Rules.Controls.ShowGhostPiece) return;

                _activePiece = value;
                CurrentColor = _activePiece.GhostPieceColor;
            }
        }

        public Color CurrentColor
        {
            get => _currentColor;
            private set
            {
                var newColor = value;
                if (!_colorGhostPiece) 
                    newColor = _defaultColor;
                if (_currentColor == newColor)
                    return;
                _currentColor = newColor;
                ColorChanged?.Invoke(_currentColor);
            }
        }

        public IEnumerable<Vector3> BlockPositions =>
            Blocks.Select(block => block.transform.position);

        public string Type => "ghost";
        public event Action<Color> ColorChanged;
        public event Action Rendered;
        
        private void Awake()
        {
            for (var i = 0; i < Blocks.Count; i++)
            {
                var block = Blocks[i];
                block.Board = _board;
                block.BlockNumber = (uint)i;
            }
        }

        private void Start()
        {
            if (!_settings.Rules.Controls.ShowGhostPiece)
                gameObject.SetActive(false);

            ColorGhostPieceApplier.ColorGhostPieceChanged += ChangeColoring;
            ChangeColoring(AppSettings.Gameplay.ColorGhostPiece);
        }

        private void ChangeColoring(bool colorGhostPiece)
        {
            _colorGhostPiece = colorGhostPiece;
            CurrentColor = colorGhostPiece ? _activePiece.GhostPieceColor : _defaultColor;
        }
        
        public void Render()
        {
            if (!_settings.Rules.Controls.ShowGhostPiece) return;
            
            transform.position = ActivePiece.transform.position;
            for (var i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].transform.position = ActivePiece.Blocks[i].transform.position;
                Blocks[i].transform.rotation = ActivePiece.Blocks[i].transform.rotation;
            }

            var moveVector = Vector2Int.down;
            while (_board.CanPlace(Blocks, moveVector)) moveVector += Vector2Int.down;
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
    }
}