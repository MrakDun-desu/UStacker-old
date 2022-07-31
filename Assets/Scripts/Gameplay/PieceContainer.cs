using Blockstacker.Gameplay.Pieces;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class PieceContainer : MonoBehaviour
    {
        public const string USED_HOLD_TYPE = "usedHold";

        public const float Width = 6;
        public const float Height = 3;
        private Piece _piece;
        private bool _pieceIsNull = true;

        public Piece Piece
        {
            get => _piece;
            private set
            {
                _pieceIsNull = value == null;
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
            _piece.Rotate(360);
        }

        public void UnmarkUsed()
        {
            if (_pieceIsNull) 
                return;
            
            _piece.RevertType();
            // rotating to reset state
            _piece.Rotate(360);
        }

        public Piece SwapPiece(Piece newPiece)
        {
            var temp = Piece;
            Piece = newPiece;

            if (_pieceIsNull) return temp;

            var pieceTransform = _piece.transform;
            pieceTransform.SetParent(transform);
            pieceTransform.localPosition = new Vector3(_piece.ContainerOffset.x + Width * .5f,
                _piece.ContainerOffset.y + Height * .5f);
            pieceTransform.localScale = new Vector3(1, 1, 1);
            pieceTransform.rotation = transform.rotation;
            return temp;
        }
    }
}