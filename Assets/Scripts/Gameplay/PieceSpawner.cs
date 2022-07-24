using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;
using UnityEngine;
using UnityEngine.Pool;

namespace Blockstacker.Gameplay
{
    public class PieceSpawner : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _settings;
        [SerializeField] private Board _board;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private GameManager _manager;
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private WarningPiece _warningPiece;

        public IRandomizer Randomizer;
        public List<PieceContainer> PreviewContainers = new();

        private PiecePreviews _previews;
        private ObjectPool<Piece>[] _piecePools;

        public void InitContainers()
        {
            _previews = new PiecePreviews(PreviewContainers);
        }

        public void PrespawnPieces()
        {
            foreach (var nextIndex in PreviewContainers.Select(_ => Randomizer.GetNextPiece()))
            {
                var nextPiece = GetPieceFromPool(nextIndex);
                nextPiece.SetBoard(_board);
                _previews.AddPiece(nextPiece);
            }
        }

        public void SpawnPiece() => SpawnPiece(0d);

        public void SpawnPiece(double spawnTime)
        {
            var nextIndex = Randomizer.GetNextPiece();
            var swappedPiece = GetPieceFromPool(nextIndex);
            swappedPiece.SetBoard(_board);
            var nextPiece = _previews.AddPiece(swappedPiece);

            SpawnPiece(nextPiece, spawnTime);
        }

        public void SpawnPiece(Piece piece, double spawnTime)
        {
            var boardTransform = _board.transform;
            var piecePos = new Vector3(
                (int) (_settings.Rules.BoardDimensions.BoardWidth / 2u),
                (int) _settings.Rules.BoardDimensions.PieceSpawnHeight,
                boardTransform.position.z
            );
            var pieceTransform = piece.transform;
            pieceTransform.SetParent(boardTransform);
            pieceTransform.localScale = Vector3.one;
            pieceTransform.localPosition = piecePos + new Vector3(piece.SpawnOffset.x, piece.SpawnOffset.y);

            _inputProcessor.ActivePiece = piece;

            var rotationSystem = _settings.Rules.Controls.ActiveRotationSystem;
            var rotation = rotationSystem.GetKickTable(piece.Type).StartState;
            pieceTransform.Rotate(Vector3.forward, (float) rotation);

            _warningPiece.SetPiece(_previews.GetFirstPiece());

            var nextPiece = "";
            if (AppSettings.Sound.HearNextPieces)
                nextPiece = _previews.GetFirstPieceType();

            _mediator.Send(new PieceSpawnedMessage
            {
                SpawnedPiece = piece.Type, NextPiece = nextPiece, Time = spawnTime
            });

            if (!_board.CanPlace(piece))
                _manager.LoseGame();
        }

        public void EmptyAllContainers()
        {
            foreach (var piece in PreviewContainers.Select(container => container.SwapPiece(null))
                         .Where(piece => piece != null)) Destroy(piece.gameObject);
        }

        public void SetAvailablePieces(IEnumerable<Piece> pieces)
        {
            var blockCount = _settings.Rules.BoardDimensions.BoardHeight * _settings.Rules.BoardDimensions.BoardWidth;
            var defaultCapacity = (int) (blockCount / 25u);
            var maxSize = (int) (blockCount / 3u);

            _piecePools = pieces.Select(piece =>
            {
                return new ObjectPool<Piece>(
                    () => Instantiate(piece),
                    p => p.ResetState(),
                    p => p.gameObject.SetActive(false),
                    p => Destroy(p.gameObject),
                    true,
                    defaultCapacity,
                    maxSize
                );
            }).ToArray();
        }

        private Piece GetPieceFromPool(int nextIndex)
        {
            var output = _piecePools[nextIndex].Get();
            output.SourcePool = _piecePools[nextIndex];
            return output;
        }
    }
}