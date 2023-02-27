using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UStacker.Common.Extensions;
using UStacker.Gameplay.GarbageGeneration;
using UStacker.Gameplay.Pieces;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.Appliers;
using UStacker.GlobalSettings.BlockSkins;
using UStacker.GlobalSettings.Enums;
using UnityEngine;

namespace UStacker.Gameplay.Blocks
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockSkin : MonoBehaviour
    {
        [SerializeField] private SkinRecord _skinRecord;
        [SerializeField] private SpriteRenderer _renderer;
        private List<SpriteRecord> _currentSprites = new();

        private float _switchFrameTime;
        private float _visibility = 1;
        private Coroutine _animationCoroutine;

        public float Visibility
        {
            get => _visibility;
            set
            {
                _visibility = Mathf.Clamp01(value);
                _renderer.color = _renderer.color.WithAlpha(_visibility);
            }
        }

        public IBlockCollection BlockCollection { get; set; }

        public Board Board { get; set; }

        public SkinRecord SkinRecord
        {
            get => _skinRecord;
            set
            {
                _skinRecord = value;
                RefreshSkin();
            }
        }

        private void HandleSpritesChanged()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
            
            switch (_currentSprites.Count)
            {
                case 0:
                    return;
                case 1:
                    _renderer.sprite = _currentSprites[0].Sprite;
                    return;
            }

            if (SkinRecord.AnimationFps <= 0) return;

            _animationCoroutine = StartCoroutine(AnimationCor());
        }

        private IEnumerator AnimationCor()
        {
            var currentSpriteIndex = 0;
            while (true)
            {
                currentSpriteIndex %= _currentSprites.Count;
                _renderer.sprite = _currentSprites[currentSpriteIndex].Sprite;
                yield return new WaitForSeconds(_switchFrameTime);
                currentSpriteIndex++;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        public void RefreshSkin()
        {
            _switchFrameTime = 1f / SkinRecord.AnimationFps;
            _renderer.sortingOrder = SkinRecord.Layer;

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
                HandleSpritesChanged();
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
                    case GarbageLayer garbageLayer:
                        garbageLayer.BlocksAdded += PickConnectedPart;
                        break;
                }

                if (BlockCollection is not BoardGrid)
                    Board.LinesCleared += PickConnectedPart;
            }

            switch (BlockCollection)
            {
                case GhostPiece ghostPiece:
                    GhostPieceVisibilityApplier.VisibilityChanged += ChangeAlpha;

                    if (!AppSettings.Gameplay.ColorGhostPiece) return;
                    ghostPiece.ColorChanged += ChangeColor;
                    ChangeColor(ghostPiece.CurrentColor);
                    break;
                case BoardGrid:
                    GridVisibilityApplier.VisibilityChanged += ChangeAlpha;
                    break;
            }
        }

        private Vector3 RelativePos(Vector2Int pos)
        {
            var boardScale = Board.transform.localScale;
            var myTransform = transform;
            var myPos = myTransform.position;
            return myPos + (myTransform.right * (pos.x * boardScale.x) + myTransform.up * (pos.y * boardScale.y));
        }

        private bool MyPieceInPos(Vector2Int pos)
        {
            var checkedPos = RelativePos(pos);
            return BlockCollection.BlockPositions.Any(worldPos => AreClose(worldPos, checkedPos));
        }

        private bool AreClose(Vector3 pos1, Vector3 pos2)
        {
            return (pos1 - pos2).sqrMagnitude < Mathf.Pow(Board.transform.localScale.x, 2) * .5f;
        }

        public void UnregisterEvents()
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
                case GarbageLayer garbageLayer:
                    garbageLayer.BlocksAdded -= PickConnectedPart;
                    break;
                case BoardGrid:
                    GridVisibilityApplier.VisibilityChanged -= ChangeAlpha;
                    break;
            }
        }

        #region Event subscriber functions

        private void PickConnectedPart()
        {
            var edges = Edges.None;
            if (!MyPieceInPos(Vector2Int.right))
                edges |= Edges.Right;
            if (!MyPieceInPos(Vector2Int.left))
                edges |= Edges.Left;
            if (!MyPieceInPos(Vector2Int.up))
                edges |= Edges.Top;
            if (!MyPieceInPos(Vector2Int.down))
                edges |= Edges.Bottom;

            if (!MyPieceInPos(new Vector2Int(1, 1)) && MyPieceInPos(Vector2Int.up) && MyPieceInPos(Vector2Int.right))
                edges |= Edges.TopRight;
            if (!MyPieceInPos(new Vector2Int(-1, 1)) && MyPieceInPos(Vector2Int.up) && MyPieceInPos(Vector2Int.left))
                edges |= Edges.TopLeft;
            if (!MyPieceInPos(new Vector2Int(1, -1)) && MyPieceInPos(Vector2Int.down) && MyPieceInPos(Vector2Int.right))
                edges |= Edges.BottomRight;
            if (!MyPieceInPos(new Vector2Int(-1, -1)) && MyPieceInPos(Vector2Int.down) && MyPieceInPos(Vector2Int.left))
                edges |= Edges.BottomLeft;

            var connectedSprite = SkinRecord.ConnectedSprites.Find(sprite => sprite.Edges == edges) ??
                                  SkinRecord.ConnectedSprites.Find(sprite => sprite.Edges == Edges.None);
            if (connectedSprite is null)
                return;

            _currentSprites = connectedSprite.Sprites;
            HandleSpritesChanged();
        }

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
    }
}