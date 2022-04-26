using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;

namespace Blockstacker.Gameplay
{
    public class PiecePreviews
    {
        private readonly IList<PieceContainer> _containers;
        private Piece lastPiece;

        public PiecePreviews(IList<PieceContainer> containers)
        {
            _containers = containers;
        }
        
        public Piece AddPiece(Piece newPiece)
        {
            if (_containers.Count > 0)
                return _containers.Aggregate(newPiece, (current, container) => container.SwapPiece(current));
            var temp = lastPiece;
            lastPiece = newPiece;
            return temp;
        }
        
    }
}