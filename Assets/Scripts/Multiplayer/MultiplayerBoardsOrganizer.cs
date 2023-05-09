
/************************************
MultiplayerBoardsOrganizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
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

        private readonly List<MultiplayerBoard> _boards = new();
        private Vector2 _basicPadding;
        private Vector2 _boardSize;
        private Vector2 _fullPadding;
        private MultiplayerBoard _mainBoard;

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set
            {
                _boardSize = new Vector2(value.BoardDimensions.BoardWidth, value.BoardDimensions.BoardHeight);
                _fullPadding = new Vector2(_multiplayerBoardPadding, _multiplayerBoardPadding);
                _basicPadding = _fullPadding;
                if (value.Controls.AllowHold)
                    _fullPadding.x += PieceContainer.WIDTH;
                if (value.General.NextPieceCount > 1)
                    _fullPadding.x += PieceContainer.WIDTH;
            }
        }

        public void SetMainBoard(MultiplayerBoard board)
        {
            if (_mainBoard is not null)
                _boards.Add(_mainBoard);

            _mainBoard = board;
            _mainBoard.SetDetailLevel(BoardDetailLevel.Full);
            Reorganize();
        }

        public void UnsetMainBoard(bool reorganize = true)
        {
            if (_mainBoard is null)
                return;

            _boards.Add(_mainBoard);
            _mainBoard = null;
            if (reorganize)
                Reorganize();
        }

        public void AddBoard(MultiplayerBoard newBoard)
        {
            _boards.Add(newBoard);
        }

        public void RemoveBoard(MultiplayerBoard board)
        {
            if (_mainBoard == board)
                UnsetMainBoard(false);

            _boards.Remove(board);
            Reorganize();
        }

        public void Reorganize()
        {
            var boardCount = _boards.Count + (_mainBoard is not null ? 1 : 0);
            var boardDetailLevel = boardCount switch
            {
                <= 2 => BoardDetailLevel.Full,
                <= 8 => BoardDetailLevel.Medium,
                _ => BoardDetailLevel.Basic
            };

            foreach (var board in _boards)
                board.SetDetailLevel(boardDetailLevel);

            var currentPosition = new Vector2(-_boardSize.x - _fullPadding.x * .5f,
                _boardSize.y * -.5f);

            var maxX = _boardSize.x + _fullPadding.x * .5f;

            if (_mainBoard is not null)
            {
                _mainBoard.transform.position = currentPosition;
                currentPosition.x += _boardSize.x + _fullPadding.x;
            }

            var positionAddition = boardDetailLevel == BoardDetailLevel.Basic
                ? _boardSize + _basicPadding
                : _boardSize + _fullPadding;

            switch (boardDetailLevel)
            {
                case BoardDetailLevel.Basic:
                case BoardDetailLevel.Medium:
                    if (_boards.Count <= 4)
                    {
                    }

                    break;
                case BoardDetailLevel.Full:
                    // if detail level is full, we don't need to make more rows
                    foreach (var board in _boards)
                    {
                        board.transform.position = currentPosition;
                        currentPosition.x += positionAddition.x;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
/************************************
end MultiplayerBoardsOrganizer.cs
*************************************/
