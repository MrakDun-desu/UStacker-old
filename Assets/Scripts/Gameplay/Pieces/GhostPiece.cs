using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common.Extensions;
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
            ColorGhostPieceApplier.ColorGhostPieceChanged += ChangeColoring;

            if (_settings.Rules.Controls.ShowGhostPiece) return;
            foreach (var block in Blocks) block.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GhostPieceVisibilityApplier.VisibilityChanged -= ChangeRendererAlphas;
            ColorGhostPieceApplier.ColorGhostPieceChanged -= ChangeColoring;
        }

        private void ChangeColoring(bool colorThis)
        {
            if (!colorThis) return;
            foreach (var spriteRenderer in _blockRenderers)
                spriteRenderer.color = spriteRenderer.color.WithValue(Color.white);
        }

        private void ChangeRendererAlphas(float newAlpha)
        {
            foreach (var spriteRenderer in _blockRenderers)
                spriteRenderer.color = spriteRenderer.color.WithAlpha(newAlpha);
        }

        public void SetActivePiece(Piece value)
        {
            if (!_settings.Rules.Controls.ShowGhostPiece) return;

            _activePiece = value;

            if (!AppSettings.Gameplay.ColorGhostPiece) return;
            
            foreach (var spriteRenderer in _blockRenderers)
                spriteRenderer.color = spriteRenderer.color.WithValue(_activePiece.GhostPieceColor);
        }

        public void Render()
        {
            if (!_settings.Rules.Controls.ShowGhostPiece) return;
            transform.position = _activePiece.transform.position;
            for (var i = 0; i < Blocks.Count; i++) Blocks[i].position = _activePiece.Blocks[i].transform.position;

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
        }
    }
}