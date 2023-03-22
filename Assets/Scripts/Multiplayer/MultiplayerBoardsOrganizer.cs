using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Multiplayer
{
    public class MultiplayerBoardsOrganizer : MonoBehaviour
    {
        private readonly List<MultiplayerBoard> _boards = new();
        public Vector2Int BoardSize { get; set; }

        public void AddBoard(MultiplayerBoard newBoard)
        {
            _boards.Add(newBoard);
            Reorganize();
        }

        public void RemoveBoard(MultiplayerBoard board)
        {
            _boards.Remove(board);
            Reorganize();
        }

        private void Reorganize()
        {
        }
    }
}