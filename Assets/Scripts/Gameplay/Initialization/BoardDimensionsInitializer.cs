﻿using System.Text;
using UStacker.Gameplay.Blocks;
using UStacker.Gameplay.Pieces;
using UStacker.GameSettings;
using UnityEngine;

namespace UStacker.Gameplay.Initialization
{
    public class BoardDimensionsInitializer : InitializerBase
    {
        private readonly Board _board;
        private readonly GameObject _boardBackground;
        private readonly BoardGrid _boardGrid;
        private readonly Camera _camera;
        private readonly BlockBase _gridBlock;
        private readonly RectTransform _statCanvasTransform;

        public BoardDimensionsInitializer(
            StringBuilder errorBuilder, GameSettingsSO.SettingsContainer gameSettings,
            Board board,
            GameObject boardBackground,
            BlockBase gridBlock,
            BoardGrid boardGrid,
            RectTransform statsCanvasTransform,
            Camera camera
        )
            : base(errorBuilder, gameSettings)
        {
            _board = board;
            _boardBackground = boardBackground;
            _gridBlock = gridBlock;
            _boardGrid = boardGrid;
            _camera = camera;
            _statCanvasTransform = statsCanvasTransform;
        }

        public override void Execute()
        {
            var boardDimensions = _gameSettings.BoardDimensions;
            _board.Width = _gameSettings.BoardDimensions.BoardWidth;
            _board.Height = _gameSettings.BoardDimensions.BoardHeight;
            _board.LethalHeight = _gameSettings.BoardDimensions.LethalHeight;
            _boardBackground.transform.localScale = new Vector3(
                boardDimensions.BoardWidth,
                boardDimensions.BoardHeight,
                1
            );

            _statCanvasTransform.sizeDelta =
                new Vector2(boardDimensions.BoardWidth + 200f, boardDimensions.BoardHeight + 200f);

            var gridTransform = _boardGrid.transform;
            gridTransform.SetParent(_board.transform);
            gridTransform.localPosition = Vector3.zero;
            gridTransform.localScale = Vector3.one;

            for (var y = 0; y < boardDimensions.BoardHeight; y++)
            for (var x = 0; x < boardDimensions.BoardWidth; x++)
            {
                var gridBlock = Object.Instantiate(
                    _gridBlock,
                    _boardGrid.transform,
                    false
                );

                var blockTransform = gridBlock.transform;
                blockTransform.localPosition = new Vector3(x + .5f, y + .5f, blockTransform.localPosition.z);

                _boardGrid.AddBlock(gridBlock);
            }

            _camera.orthographicSize =
                boardDimensions.BoardHeight * .5f + _gameSettings.BoardDimensions.BoardPadding;
        }
    }
}