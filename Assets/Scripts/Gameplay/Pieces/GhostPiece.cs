using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Enums;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Appliers;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class GhostPiece : MonoBehaviour
    {
        public List<Transform> Blocks = new();
        [SerializeField] private Board _board;
        [SerializeField] private GameSettingsSO _settings;
        
        [Header("Piece type colors")]
        [SerializeField] private Color IPieceColor = CreateColor.From255Range(3, 250, 252);
        [SerializeField] private Color TPieceColor = CreateColor.From255Range(159, 3, 252);
        [SerializeField] private Color OPieceColor = CreateColor.From255Range(252, 254, 3);
        [SerializeField] private Color LPieceColor = CreateColor.From255Range(252, 149, 3);
        [SerializeField] private Color JPieceColor = CreateColor.From255Range(3, 5, 252);
        [SerializeField] private Color SPieceColor = CreateColor.From255Range(3, 254, 30);
        [SerializeField] private Color ZPieceColor = CreateColor.From255Range(252, 3, 16);

        private readonly List<SpriteRenderer> _blockRenderers = new();
        private Piece _activePiece;

        private void Awake()
        {
            var newAlpha = AppSettings.Gameplay.GhostPieceVisibility;
            foreach (var spriteRenderer in Blocks.Select(block => block.GetComponentInChildren<SpriteRenderer>()))
            {
                spriteRenderer.color = spriteRenderer.color.WithAlpha(newAlpha);
                _blockRenderers.Add(spriteRenderer);
            }

            GhostPieceVisibilityApplier.VisibilityChanged += ChangeRendererAlphas;
            ColorGhostPieceApplier.ColorGhostPieceChanged += colorThis =>
            {
                if (!colorThis) return;
                foreach (var spriteRenderer in _blockRenderers)
                {
                    spriteRenderer.color = spriteRenderer.color.WithValue(Color.white);
                }
            };

            if (_settings.Rules.Controls.ShowGhostPiece) return;
            foreach (var block in Blocks)
            {
                block.gameObject.SetActive(false);
            }
        }

        private void ChangeRendererAlphas(float newAlpha)
        {
            foreach (var spriteRenderer in _blockRenderers)
            {
                spriteRenderer.color = spriteRenderer.color.WithAlpha(newAlpha);
            } 
        }
        
        private void ColorGhostPiece(PieceType pieceType)
        {
            var newColor = pieceType switch
            {
                PieceType.IPiece => IPieceColor,
                PieceType.TPiece => TPieceColor,
                PieceType.OPiece => OPieceColor,
                PieceType.LPiece => LPieceColor,
                PieceType.JPiece => JPieceColor,
                PieceType.SPiece => SPieceColor,
                PieceType.ZPiece => ZPieceColor,
                _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null)
            };

            foreach (var spriteRenderer in _blockRenderers)
            {
                spriteRenderer.color = spriteRenderer.color.WithValue(newColor);
            }
        }
        
        public void SetActivePiece(Piece value)
        {
            if (!_settings.Rules.Controls.ShowGhostPiece) return;
            
            _activePiece = value;

            if (!AppSettings.Gameplay.ColorGhostPiece) return;
            ColorGhostPiece(_activePiece.PieceType);
        }

        public void Render()
        {
            if (!_settings.Rules.Controls.ShowGhostPiece) return;
            transform.position = _activePiece.transform.position;
            for (var i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].position = _activePiece.Blocks[i].transform.position;
            }

            var moveVector = Vector2Int.down;
            while (_board.CanPlace(Blocks, moveVector))
            {
                moveVector += Vector2Int.down;
            }
            moveVector -= Vector2Int.down;

            var pieceTransform = transform;
            var piecePosition = pieceTransform.localPosition;
            piecePosition = new Vector3(
                piecePosition.x + moveVector.x,
                piecePosition.y + moveVector.y,
                piecePosition.z);
            pieceTransform.localPosition = piecePosition;
        }
    }
}