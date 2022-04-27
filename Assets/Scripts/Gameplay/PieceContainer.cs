using Blockstacker.Gameplay.Pieces;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class PieceContainer : MonoBehaviour
    {
        private Piece _piece;
        public const float Width = 6;
        public const float Height = 3;

        public Piece SwapPiece(Piece newPiece)
        {
            var temp = _piece;
            _piece = newPiece;
            
            if (newPiece == null)
            {
                return temp;
            }
            
            var pieceTransform = _piece.transform;
            pieceTransform.SetParent(transform);
            pieceTransform.localPosition = new Vector3(_piece.ContainerOffset.x + Width * .5f, _piece.ContainerOffset.y + Height * .5f);
            pieceTransform.rotation = Quaternion.identity;
            return temp;
        }

    }
}