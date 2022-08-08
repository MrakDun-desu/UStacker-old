using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;

namespace Blockstacker.Gameplay
{
    public class PiecePreviews
    {
        private readonly List<PieceContainer> _containers;

        public PiecePreviews(List<PieceContainer> containers)
        {
            _containers = containers;
            _containers.Sort((x, y) => x.transform.position.y > y.transform.position.y ? 1 : -1);
        }

        public Piece AddPiece(Piece newPiece)
        {
            return _containers.Count > 0
                ? _containers.Aggregate(newPiece, (current, container) => container.SwapPiece(current))
                : newPiece;
        }

        public string GetFirstPieceType()
        {
            return _containers.Count > 0 ? _containers[^1].PieceType : "";
        }

        public Piece GetFirstPiece()
        {
            return _containers.Count > 0 ? _containers[^1].Piece : null;
        }
    }
}