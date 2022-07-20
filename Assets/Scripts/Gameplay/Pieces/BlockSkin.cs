using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.GlobalSettings.BlockSkins;
using Blockstacker.GlobalSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Pieces
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockSkin : MonoBehaviour
    {
        [SerializeField] private SkinRecord _skinRecord;
        
        private SpriteRenderer _renderer;
        private float _switchFrameTime;
        private List<SpriteRecord> _currentSprites = new();
        
        public Piece Piece { get; set; }
        public Board Board { get; set; }
        
        public SkinRecord SkinRecord
        {
            get => _skinRecord;
            set {
                _skinRecord = value;
                RefreshSkin();
            }
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            switch (_currentSprites.Count)
            {
                case 0:
                    return;
                case 1:
                    _renderer.sprite = _currentSprites[0].Sprite;
                    return;
            }

            if (SkinRecord.AnimationFps == 0) return;

            var newSpriteIndex = Mathf.FloorToInt(Time.realtimeSinceStartup / _switchFrameTime);
            newSpriteIndex %= _currentSprites.Count;
            newSpriteIndex = newSpriteIndex < 0 ? 0 : newSpriteIndex;
            _renderer.sprite = _currentSprites[newSpriteIndex].Sprite;
        }

        private void RefreshSkin()
        {
            _switchFrameTime = 1f / SkinRecord.AnimationFps;
            _renderer.sortingOrder = (int) SkinRecord.Layer;
            
            if (SkinRecord.RotateWithPiece) return;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            Piece.Rotated += ResetRotation;

            if (!SkinRecord.IsConnected)
            {
                _currentSprites = SkinRecord.Sprites;
                Update();
            }
            else
            {
                PickConnectedPart();

                Piece.Rotated += PickConnectedPart;
                Board.LinesCleared += PickConnectedPart;
            }
        }

        private void PickConnectedPart()
        {
            var edges = ConnectedEdges.None;
            if (!MyPieceInPos(Vector2Int.right))
                edges |= ConnectedEdges.Right;
            if (!MyPieceInPos(Vector2Int.left))
                edges |= ConnectedEdges.Left;
            if (!MyPieceInPos(Vector2Int.up))
                edges |= ConnectedEdges.Top;
            if (!MyPieceInPos(Vector2Int.down))
                edges |= ConnectedEdges.Bottom;
            
            if (!MyPieceInPos(new Vector2Int(1,1)) && MyPieceInPos(Vector2Int.up) && MyPieceInPos(Vector2Int.right))
                edges |= ConnectedEdges.TopRight;
            if (!MyPieceInPos(new Vector2Int(-1,1)) && MyPieceInPos(Vector2Int.up) && MyPieceInPos(Vector2Int.left))
                edges |= ConnectedEdges.TopLeft;
            if (!MyPieceInPos(new Vector2Int(1,-1)) && MyPieceInPos(Vector2Int.down) && MyPieceInPos(Vector2Int.right))
                edges |= ConnectedEdges.BottomRight;
            if (!MyPieceInPos(new Vector2Int(-1, -1)) && MyPieceInPos(Vector2Int.down) && MyPieceInPos(Vector2Int.left))
                edges |= ConnectedEdges.BottomLeft;

            var connectedSprite = SkinRecord.ConnectedSprites.Find(sprite => sprite.ConnectedEdges == edges);
            if (connectedSprite is null)
                return;
            
            _currentSprites = connectedSprite.Sprites;
            Update();
        }

        private Vector3 RelativePos(Vector2Int pos)
        {
            var boardTransform = Board.transform;
            var boardScale = boardTransform.localScale;
            var myPos = transform.position;
            return myPos + (boardTransform.right * (pos.x * boardScale.x) + boardTransform.up * (pos.y * boardScale.y));
        }

        private bool MyPieceInPos(Vector2Int pos)
        {
            var checkedPos = Board.WorldSpaceToBoardPosition(RelativePos(pos));
            return Piece.Blocks.Any(block => Board.WorldSpaceToBoardPosition(block.transform.position) == checkedPos);
        }
        
        private void ResetRotation()
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        private void OnDestroy()
        {
            if (Piece == null) return;
            
            Piece.Rotated -= ResetRotation;
            Piece.Rotated -= PickConnectedPart;
            Board.LinesCleared -= PickConnectedPart;
        }

    }
}