using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Pieces;

namespace UStacker.Gameplay
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

        public Piece GetFirstPiece()
        {
            return _containers.Count > 0 ? _containers[^1].Piece : null;
        }
    }
}