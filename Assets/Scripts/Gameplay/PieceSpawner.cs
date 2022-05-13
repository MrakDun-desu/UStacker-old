using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class PieceSpawner : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _settings;
        [SerializeField] private Board _board;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private GameManager _manager;

        public Piece[] AvailablePieces;
        public IRandomizer Randomizer;
        public List<PieceContainer> PreviewContainers = new();

        private PiecePreviews _previews;

        public void InitContainers()
        {
            _previews = new PiecePreviews(PreviewContainers);
        }

        public void PrespawnPieces()
        {
            foreach (var _ in PreviewContainers)
            {
                var nextIndex = Randomizer.GetNextPiece();
                _previews.AddPiece(Instantiate(AvailablePieces[nextIndex]));
            }
        }

        public void SpawnPiece()
        {
            var nextIndex = Randomizer.GetNextPiece();
            var nextPiece = _previews.AddPiece(Instantiate(AvailablePieces[nextIndex]));

            SpawnPiece(nextPiece);
        }

        public void SpawnPiece(Piece piece)
        {
            var boardTransform = _board.transform;
            var piecePos = new Vector3(
                (int) _settings.Rules.BoardDimensions.BoardWidth / 2,
                (int) _settings.Rules.BoardDimensions.PieceSpawnHeight,
                boardTransform.position.z
            );
            var pieceTransform = piece.transform;
            pieceTransform.SetParent(null);
            pieceTransform.localScale = boardTransform.localScale;
            pieceTransform.SetParent(boardTransform);
            pieceTransform.localPosition = piecePos + new Vector3(piece.SpawnOffset.x, piece.SpawnOffset.y);

            _inputProcessor.ActivePiece = piece;

            if (!_board.CanPlace(piece)) 
                _manager.LoseGame();
        }
        
        public void EmptyAllContainers()
        {
            foreach (var piece in PreviewContainers.Select(container => container.SwapPiece(null)).Where(piece => piece != null))
            {
                Destroy(piece.gameObject);
            }
        }
    }
}