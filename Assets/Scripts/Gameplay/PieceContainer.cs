using Blockstacker.Gameplay.Pieces;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class PieceContainer : MonoBehaviour
    {
        public const float Width = 6;
        public const float Height = 3;
        private Piece _piece;

        public Piece SwapPiece(Piece newPiece)
        {
            var temp = _piece;
            _piece = newPiece;

            if (newPiece == null) return temp;

            var pieceTransform = _piece.transform;
            pieceTransform.SetParent(transform);
            pieceTransform.localPosition = new Vector3(_piece.ContainerOffset.x + Width * .5f,
                _piece.ContainerOffset.y + Height * .5f);
            pieceTransform.localScale = new Vector3(1, 1, 1);
            pieceTransform.rotation = transform.rotation;
            return temp;
        }

        public string GetPieceType()
        {
            return _piece.PieceType;
        } 
    }
}