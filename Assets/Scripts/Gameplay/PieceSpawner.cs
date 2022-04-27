using System;
using System.Collections.Generic;
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

        public Piece[] AvailablePieces;
        public IRandomizer Randomizer;
        public List<PieceContainer> PreviewContainers = new();

        private PiecePreviews _previews;

        public void Init()
        {
            _previews = new PiecePreviews(PreviewContainers);

            foreach (var _ in PreviewContainers)
            {
                var nextIndex = Randomizer.GetNextPiece();
                _previews.AddPiece(Instantiate(AvailablePieces[nextIndex]));
            }

            SpawnPiece();
        }

        public void SpawnPiece()
        {
            var nextIndex = Randomizer.GetNextPiece();
            var nextPiece = _previews.AddPiece(Instantiate(AvailablePieces[nextIndex]));

            var boardTransform = _board.transform;
            var piecePos = new Vector3(
                (int) _settings.Rules.BoardDimensions.BoardWidth / 2,
                (int) _settings.Rules.BoardDimensions.PieceSpawnHeight,
                boardTransform.position.z
            );
            var pieceTransform = nextPiece.transform;
            pieceTransform.SetParent(boardTransform);
            pieceTransform.localPosition = piecePos + new Vector3(nextPiece.SpawnOffset.x, nextPiece.SpawnOffset.y);
            pieceTransform.localScale = boardTransform.localScale;

            _inputProcessor.ActivePiece = nextPiece;
        }
    }
}