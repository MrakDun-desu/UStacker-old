using System.Text;
using Blockstacker.GameSettings;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesBoardDimensionsInitializer : InitializerBase
    {
        private readonly Board _board;
        private readonly GameObject _boardBackground;
        private readonly GameObject _gridPiece;
        private readonly Camera _camera;

        public RulesBoardDimensionsInitializer(
            StringBuilder errorBuilder,
            GameSettingsSO gameSettings,
            Board board,
            GameObject boardBackground,
            GameObject gridPiece,
            Camera camera
        )
            : base(errorBuilder, gameSettings)
        {
            _board = board;
            _boardBackground = boardBackground;
            _gridPiece = gridPiece;
            _camera = camera;
        }

        public override void Execute()
        {
            var boardDimensions = _gameSettings.Rules.BoardDimensions;
            _board.Width = _gameSettings.Rules.BoardDimensions.BoardWidth;
            _board.Height = _gameSettings.Rules.BoardDimensions.BoardHeight;
            _board.LethalHeight = _gameSettings.Rules.BoardDimensions.LethalHeight;
            _boardBackground.transform.localScale = new Vector3(
                boardDimensions.BoardWidth,
                boardDimensions.BoardHeight,
                1
            );

            var boardGrid = new GameObject("Grid");
            boardGrid.transform.SetParent(_board.transform);

            for (var y = 0; y < boardDimensions.BoardHeight; y++)
            {
                for (var x = 0; x < boardDimensions.BoardWidth; x++)
                {
                    var gridPiece = Object.Instantiate(_gridPiece,
                        boardGrid.transform,
                        false);
                    gridPiece.transform.localPosition = new Vector3(
                        x, y, gridPiece.transform.localPosition.z);
                }
            }

            _camera.orthographicSize =
                boardDimensions.BoardHeight * .5f + _gameSettings.Rules.BoardDimensions.BoardPadding;
            _camera.transform.position = new Vector3(
                _gameSettings.Rules.BoardDimensions.BoardWidth * .5f,
                _gameSettings.Rules.BoardDimensions.BoardHeight * .5f,
                _camera.transform.position.z);
        }
    }
}