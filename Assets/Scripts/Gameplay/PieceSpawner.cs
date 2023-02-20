using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Pieces;
using UStacker.Gameplay.Randomizers;
using UStacker.GameSettings;
using UStacker.GlobalSettings;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UStacker.Gameplay.InputProcessing;

namespace UStacker.Gameplay
{
    public class PieceSpawner : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Board _board;
        [SerializeField] private InputProcessor _inputProcessor;
        [FormerlySerializedAs("_manager")] [SerializeField]
        private GameStateManager _stateManager;
        [SerializeField] private WarningPiece _warningPiece;
        [SerializeField] private Mediator _mediator;
        
        private readonly Dictionary<string, ObjectPool<Piece>> _piecePools = new();
        private bool _containersEmpty = true;
        private int _defaultPoolCapacity;
        private List<PieceContainer> _previewContainers;
        private PiecePreviews _previews;
        private GameSettingsSO.SettingsContainer _settings;

        public IRandomizer Randomizer { get; set; }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set => _settings = value;
        }

        public void SetPreviewContainers(List<PieceContainer> previewContainers)
        {
            _previewContainers = previewContainers;
            _previews = new PiecePreviews(_previewContainers);
        }

        public void PrespawnPieces()
        {
            foreach (var nextPieceType in _previewContainers.Select(_ => Randomizer.GetNextPiece()))
            {
                var nextPiece = _piecePools[nextPieceType].Get();
                nextPiece.SetBoard(_board);
                _previews.AddPiece(nextPiece);
            }

            _containersEmpty = false;
        }

        public void SpawnPiece()
        {
            SpawnPiece(0d);
        }

        public bool SpawnPiece(double spawnTime)
        {
            if (_containersEmpty) return false;

            var newPieceType = Randomizer.GetNextPiece();
            var newPiece = _piecePools[newPieceType].Get();
            newPiece.SetBoard(_board);
            var nextPiece = _previews.AddPiece(newPiece);

            SpawnPiece(nextPiece, spawnTime);
            return true;
        }

        public void SpawnPiece(Piece piece, double spawnTime)
        {
            var boardTransform = _board.transform;
            var piecePos = new Vector3(
                (int) (_settings.BoardDimensions.BoardWidth / 2u),
                (int) _settings.BoardDimensions.PieceSpawnHeight,
                boardTransform.position.z
            );

            var pieceTransform = piece.transform;
            pieceTransform.SetParent(boardTransform);
            pieceTransform.localScale = Vector3.one;
            pieceTransform.localPosition = piecePos + new Vector3(piece.SpawnOffset.x, piece.SpawnOffset.y);

            _inputProcessor.ActivePiece = piece;
            
            var rotationSystem = _settings.Controls.ActiveRotationSystem;
            var rotation = rotationSystem.GetKickTable(piece.Type).StartState;
            piece.RotationState = rotation;
            piece.Rotate((int) rotation);

            _inputProcessor.ExecuteBufferedActions(spawnTime, out var cancelSpawn);

            if (cancelSpawn) return;

            _warningPiece.SetPiece(_previews.GetFirstPiece());

            var nextPiece = AppSettings.Sound.HearNextPieces ? _previews.GetFirstPieceType() : string.Empty;

            _mediator.Send(new PieceSpawnedMessage(piece.Type, nextPiece, spawnTime));

            if (!_board.CanPlace(piece))
                _stateManager.LoseGame(spawnTime);
        }

        public void EmptyAllContainers()
        {
            foreach (var piece in _previewContainers.Select(container => container.SwapPiece(null))
                         .Where(piece => piece != null)) piece.ReleaseFromPool();

            _containersEmpty = true;
        }

        public void SetAvailablePieces(PieceDictionary pieces)
        {
            var blockCount = _settings.BoardDimensions.BoardHeight * _settings.BoardDimensions.BoardWidth;
            _defaultPoolCapacity = (int) (blockCount / 25u);
            var maxSize = (int) (blockCount / 3u);

            foreach (var piecePair in pieces)
            {
                _piecePools.Add(piecePair.Key, new ObjectPool<Piece>(
                    () => CreatePiece(piecePair.Value, piecePair.Key),
                    p => p.Activate(),
                    p => p.Deactivate(),
                    p => Destroy(p.gameObject),
                    true,
                    _defaultPoolCapacity,
                    maxSize
                ));
            }
        }

        private Piece CreatePiece(Piece piece, string pieceType)
        {
            var newPiece = Instantiate(piece);
            newPiece.SourcePool = _piecePools[pieceType];
            return newPiece;
        }
    }
}