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
        [SerializeField] private List<PieceContainer> _previewContainers;
        [SerializeField] private GameSettingsSO _settings;
        [SerializeField] private Board _board;
        [SerializeField] private InputProcessor _inputProcessor;

        public Piece[] AvailablePieces;
        public IRandomizer Randomizer;

        private PiecePreviews _previews;

        public void Init()
        {
            _previews = new PiecePreviews(_previewContainers);
            
            foreach (var _ in _previewContainers)
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
                    (int) _settings.Rules.BoardDimensions.PieceSpawnHeight,
                    (int) _settings.Rules.BoardDimensions.BoardWidth / 2,
                    boardTransform.position.z
                );
            var pieceTransform = nextPiece.transform;
            pieceTransform.localPosition = piecePos + new Vector3(nextPiece.SpawnOffset.x, nextPiece.SpawnOffset.y);
            pieceTransform.localScale = boardTransform.localScale;

            _inputProcessor.ActivePiece = nextPiece;
        }

    }
}