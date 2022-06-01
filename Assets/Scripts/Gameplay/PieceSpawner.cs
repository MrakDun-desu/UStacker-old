using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Enums;
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
            foreach (var nextIndex in PreviewContainers.Select(_ => Randomizer.GetNextPiece()))
            {
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

            var rotationSystem = _settings.Rules.Controls.ActiveRotationSystem;
            var rotation = piece.PieceType switch
            {
                PieceType.IPiece => rotationSystem.IKickTable.StartState,
                PieceType.TPiece => rotationSystem.TKickTable.StartState,
                PieceType.OPiece => rotationSystem.OKickTable.StartState,
                PieceType.JPiece => rotationSystem.JKickTable.StartState,
                PieceType.LPiece => rotationSystem.LKickTable.StartState,
                PieceType.SPiece => rotationSystem.SKickTable.StartState,
                PieceType.ZPiece => rotationSystem.ZKickTable.StartState,
                _ => throw new ArgumentOutOfRangeException()
            };
            pieceTransform.Rotate(Vector3.forward, (float) rotation);

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