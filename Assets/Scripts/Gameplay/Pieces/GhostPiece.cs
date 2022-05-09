using System;
using System.Collections.Generic;
using Blockstacker.GameSettings;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    public class GhostPiece : MonoBehaviour
    {
        public List<Transform> Blocks = new();
        [SerializeField] private GameObject _blockPrefab;
        [SerializeField] private Board _board;
        [SerializeField] private GameSettingsSO _settings;

        private Piece _activePiece;

        private void Awake()
        {
            if (_settings.Rules.Controls.ShowGhostPiece) return;
            foreach (var block in Blocks)
            {
                block.gameObject.SetActive(false);
            }
        }

        public void SetActivePiece(Piece value)
        {
            if (!_settings.Rules.Controls.ShowGhostPiece) return;
            
            _activePiece = value;
            
            while (_activePiece.Blocks.Count > value.Blocks.Count)
            {
                Blocks.Add(Instantiate(_blockPrefab).transform);
            }

            while (_activePiece.Blocks.Count < value.Blocks.Count)
            {
                Destroy(Blocks[0].gameObject);
                Blocks.RemoveAt(0);
            }
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