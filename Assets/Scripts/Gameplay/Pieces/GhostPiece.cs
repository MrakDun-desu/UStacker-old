using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class GhostPiece : MonoBehaviour, IBlockCollection
    {
        [SerializeField] private List<BlockBase> Blocks = new();
        [SerializeField] private Board _board;
        [SerializeField] private GameSettingsSO _settings;

        private Piece _activePiece;
        private Color _currentColor = Color.white;

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
                if (!AppSettings.Gameplay.ColorGhostPiece) 
                    return;
                _currentColor = value;
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
            foreach (var block in Blocks)
            {
                block.Board = _board;
            }
            if (_settings.Rules.Controls.ShowGhostPiece) return;
            gameObject.SetActive(false);
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