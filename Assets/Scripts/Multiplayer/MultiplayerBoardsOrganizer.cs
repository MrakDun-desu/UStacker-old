using System.Collections.Generic;
using UnityEngine;
using UStacker.Gameplay;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Multiplayer
{
    public class MultiplayerBoardsOrganizer : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private float _multiplayerBoardPadding = 2f;
        
        public GameSettingsSO.SettingsContainer GameSettings
        {
            set
            {
                _basicBoardSize = new Vector2(value.BoardDimensions.BoardWidth, value.BoardDimensions.BoardHeight);
                _fullBoardSize = _basicBoardSize;
                _usedPadding = new Vector2(_multiplayerBoardPadding, _multiplayerBoardPadding);
                if (value.Controls.AllowHold)
                    _usedPadding.x += PieceContainer.WIDTH;
                if (value.General.NextPieceCount > 1)
                    _usedPadding.x += PieceContainer.WIDTH;
            }
        }

        private readonly List<MultiplayerBoard> _boards = new();
        private MultiplayerBoard _mainBoard;
        private Vector2 _basicBoardSize;
        private Vector2 _fullBoardSize;
        private Vector2 _usedPadding;

        private void SetMainBoard(MultiplayerBoard board)
        {
            if (!_boards.Contains(board)) return;
            
            _mainBoard = board;
            _boards.Remove(board);
            _boards.Insert(0, board);
            Reorganize();
        }

        private void UnsetMainBoard()
        {
            if (_mainBoard is null)
                return;

            _mainBoard = null;
            Reorganize();
        }
        
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
            switch (_boards.Count)
            {
                case 2:
                    var startingPosition = new Vector2(-_fullBoardSize.x - _usedPadding.x * .5f,
                        _fullBoardSize.y * -.5f);
                    for (var i = 0; i < _boards.Count; i++)
                    {
                        var board = _boards[i];
                        board.SetDetailLevel(BoardDetailLevel.Full);
                        var positionAddition = i * Vector2.right * (_fullBoardSize.x + _usedPadding.x);

                        board.transform.position = startingPosition + positionAddition;
                    }
                    break;
                case <= 8:
                    foreach (var board in _boards)
                        board.SetDetailLevel(BoardDetailLevel.Medium);
                    
                    // TODO
                    break;
                default:
                    foreach (var board in _boards)
                        board.SetDetailLevel(BoardDetailLevel.Basic);
                    
                    // TODO
                    break;
                
            }
        }
    }
}