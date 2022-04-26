using Blockstacker.Gameplay.Pieces;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class PieceContainer : MonoBehaviour
    {
        private Piece _piece;

        public Piece SwapPiece(Piece newPiece)
        {
            var temp = _piece;
            _piece = newPiece;
            var pieceTransform = _piece.transform;
            pieceTransform.position = transform.position;
            pieceTransform.rotation = Quaternion.identity;
            pieceTransform.SetParent(transform);
            return temp;
        }

    }
}