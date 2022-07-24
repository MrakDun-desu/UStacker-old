using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Appliers;
using Blockstacker.GlobalSettings.BlockSkins;
using Blockstacker.GlobalSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Blocks
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockSkin : MonoBehaviour
    {
        [SerializeField] private SkinRecord _skinRecord;
        [SerializeField] private SpriteRenderer _renderer;
        
        private float _switchFrameTime;
        private List<SpriteRecord> _currentSprites = new();
        
        public IBlockCollection BlockCollection { get; set; }
        public Board Board { get; set; }
        
        public SkinRecord SkinRecord
        {
            get => _skinRecord;
            set {
                _skinRecord = value;
                RefreshSkin();
            }
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

        public void RefreshSkin()
        {
            _switchFrameTime = 1f / SkinRecord.AnimationFps;
            _renderer.sortingOrder = (int) SkinRecord.Layer;

            if (!SkinRecord.RotateWithPiece)
            {
                switch (BlockCollection)
                {
                    case Piece piece:
                        ResetRotation();
                        piece.Rotated += ResetRotation;
                        break;
                    case GhostPiece ghost:
                        ResetRotation();
                        ghost.Rendered += ResetRotation;
                        break;
                }
            }

            if (!SkinRecord.IsConnected)
            {
                _currentSprites = SkinRecord.Sprites;
                Update();
            }
            else
            {
                PickConnectedPart();

                switch (BlockCollection)
                {
                    case Piece piece:
                        piece.Rotated += PickConnectedPart;
                        break;
                    case GhostPiece ghost:
                        ghost.Rendered += PickConnectedPart;
                        break;
                    case WarningPiece warning:
                        warning.PieceChanged += PickConnectedPart;
                        break;
                }

                Board.LinesCleared += PickConnectedPart;
            }

            switch (BlockCollection)
            {
                case GhostPiece ghostPiece:
                {
                    _renderer.color = _renderer.color.WithAlpha(
                        AppSettings.Gameplay.GhostPieceVisibility);

                    GhostPieceVisibilityApplier.VisibilityChanged += ChangeAlpha;

                    if (!AppSettings.Gameplay.ColorGhostPiece) return;
                    ghostPiece.ColorChanged += ChangeColor;
                    ChangeColor(ghostPiece.CurrentColor);
                    break;
                }
                case BoardGrid:
                    _renderer.color = _renderer.color.WithAlpha(
                        AppSettings.Gameplay.GridVisibility);

                    GridVisibilityApplier.VisibilityChanged += ChangeAlpha;
                    break;
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
            var myTransform = transform;
            var myPos = myTransform.position;
            return myPos + (myTransform.right * (pos.x * boardScale.x) + myTransform.up * (pos.y * boardScale.y));
        }

        private bool MyPieceInPos(Vector2Int pos)
        {
            var checkedPos = Board.WorldSpaceToBoardPosition(RelativePos(pos));
            return BlockCollection.BlockPositions.Any(worldPos => Board.WorldSpaceToBoardPosition(worldPos) == checkedPos);
        }

        #region Event subscriber functions

        private void ResetRotation()
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        private void ChangeColor(Color color)
        {
            _renderer.color = _renderer.color.WithValue(color);
        }

        private void ChangeAlpha(float alpha)
        {
            _renderer.color = _renderer.color.WithAlpha(alpha);
        }
        
        #endregion

        private void OnDestroy()
        {
            if (Board != null)
                Board.LinesCleared -= PickConnectedPart;
            
            switch (BlockCollection)
            {
                case Piece piece:
                    piece.Rotated -= ResetRotation;
                    piece.Rotated -= PickConnectedPart;
                    break;
                case GhostPiece ghostPiece:
                    ghostPiece.ColorChanged -= ChangeColor;
                    ghostPiece.Rendered -= PickConnectedPart;
                    ghostPiece.Rendered -= ResetRotation;
                    GhostPieceVisibilityApplier.VisibilityChanged -= ChangeAlpha;
                    break;
                case WarningPiece warningPiece:
                    warningPiece.PieceChanged -= PickConnectedPart;
                    break;
                case BoardGrid:
                    GridVisibilityApplier.VisibilityChanged -= ChangeAlpha;
                    break;
            }
        }

    }
}