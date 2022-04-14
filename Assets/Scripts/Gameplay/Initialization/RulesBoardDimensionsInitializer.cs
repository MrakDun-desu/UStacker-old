using System.Text;
using Blockstacker.GameSettings;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesBoardDimensionsInitializer : InitializerBase
    {
        private Board _board;
        private GameObject _boardBackground;
        private GameObject _gridPiece;
        private Camera _camera;
        private GameManager _manager;

        public RulesBoardDimensionsInitializer(
            StringBuilder errorBuilder,
            GameSettingsSO gameSettings,
            Board board,
            GameObject boardBackground,
            GameObject gridPiece,
            Camera camera,
            GameManager manager)
            : base(errorBuilder, gameSettings)
        {
            _board = board;
            _boardBackground = boardBackground;
            _gridPiece = gridPiece;
            _camera = camera;
            _manager = manager;
        }

        public override void Execute()
        {
            var boardDimensions = _gameSettings.Rules.BoardDimensions;
            _board.Width = 10;
            _boardBackground.transform.localScale = new Vector3(
                boardDimensions.BoardWidth,
                boardDimensions.BoardHeight,
                1
            );

            for (var x = 0; x < boardDimensions.BoardWidth; x++)
            {
                for (var y = 0; y < boardDimensions.BoardHeight; y++)
                {
                    var gridPiece = Object.Instantiate(_gridPiece,
                        _board.gameObject.transform,
                        false);
                    gridPiece.transform.localPosition = new Vector3(
                        x, y, gridPiece.transform.localPosition.z);
                }
            }

            _camera.orthographicSize = boardDimensions.BoardHeight * .5f + 4;
        }
    }
}