using UStacker.Gameplay.Pieces;
using UnityEngine;

namespace UStacker.Gameplay
{
    public class PieceContainer : MonoBehaviour
    {
        public const string USED_HOLD_TYPE = "usedHold";

        public const float WIDTH = 6;
        public const float HEIGHT = 3;
        private Piece _piece;
        private bool _pieceIsNull = true;

        public Piece Piece
        {
            get => _piece;
            private set
            {
                _pieceIsNull = value is null;
                _piece = value;
            }
        }

        public string PieceType => _piece.Type;

        public void MarkUsed()
        {
            if (_pieceIsNull)
                return;

            _piece.Type = USED_HOLD_TYPE;
            // rotating to reset state
            _piece.Rotate(0);
        }

        public void UnmarkUsed()
        {
            if (_pieceIsNull)
                return;

            _piece.RevertType();
            // rotating to reset block skins
            _piece.Rotate(0);
        }

        public Piece SwapPiece(Piece newPiece)
        {
            var oldPiece = Piece;
            Piece = newPiece;

            if (_pieceIsNull) return oldPiece;

            var pieceTransform = _piece.transform;
            pieceTransform.SetParent(transform);
            pieceTransform.localPosition = new Vector3(_piece.ContainerOffset.x + WIDTH * .5f,
                _piece.ContainerOffset.y + HEIGHT * .5f);
            pieceTransform.localScale = new Vector3(1, 1, 1);
            pieceTransform.rotation = transform.rotation;

            // rotating to reset block skins
            Piece.Rotate(0);
            Piece.SetVisibility(1);

            return oldPiece;
        }
    }
}