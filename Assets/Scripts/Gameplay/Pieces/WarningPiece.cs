using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.GameSettings;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class WarningPiece : MonoBehaviour, IBlockCollection
    {
        [SerializeField] private BlockBase[] _blocks = Array.Empty<BlockBase>();
        [SerializeField] private Board _board;
        [SerializeField] private GameSettingsSO _settings;

        private string _currentPieceType = "";
        private bool _isEnabled = true;

        public event Action PieceChanged;

        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);
        public string Type => "warning";

        public void MakeVisible()
        {
            if (_isEnabled)
                gameObject.SetActive(true);
        }

        public void MakeInvisible() 
        {
            if (_isEnabled)
                gameObject.SetActive(false);
        }

        private void Awake()
        {
            for (var i = 0u; i < _blocks.Length; i++)
            {
                _blocks[i].BlockNumber = i;
                _blocks[i].Board = _board;
            }

            if (_settings.Rules.General.NextPieceCount <= 0) 
                _isEnabled = false;
            
            gameObject.SetActive(false);
        }

        public void SetPiece(Piece piece)
        {
            if (!enabled || !_isEnabled) return;
            if (piece.Type == _currentPieceType)
                return;

            _currentPieceType = piece.Type;

            var boardTransform = _board.transform;
            var piecePos = new Vector3(
                (int) (_settings.Rules.BoardDimensions.BoardWidth / 2u),
                (int) _settings.Rules.BoardDimensions.PieceSpawnHeight,
                boardTransform.position.z
            );
            transform.localPosition = piecePos + new Vector3(piece.SpawnOffset.x, piece.SpawnOffset.y);

            for (var i = 0; i < _blocks.Length; i++)
            {
                _blocks[i].transform.localPosition = piece.Blocks[i].transform.localPosition;
                _blocks[i].transform.rotation = piece.Blocks[i].transform.rotation;
            }

            var rotationSystem = _settings.Rules.Controls.ActiveRotationSystem;
            var rotation = rotationSystem.GetKickTable(_currentPieceType).StartState;
            transform.Rotate(Vector3.forward, (float) rotation);

            PieceChanged?.Invoke();
        }
    }
}