using System.Text;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GameSettings;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesBoardDimensionsInitializer : InitializerBase
    {
        private readonly Board _board;
        private readonly GameObject _boardBackground;
        private readonly Camera _camera;
        private readonly BlockBase _gridBlock;
        private readonly BoardGrid _boardGrid;

        public RulesBoardDimensionsInitializer(
            StringBuilder errorBuilder,
            GameSettingsSO gameSettings,
            Board board,
            GameObject boardBackground,
            BlockBase gridBlock,
            BoardGrid boardGrid,
            Camera camera
        )
            : base(errorBuilder, gameSettings)
        {
            _board = board;
            _boardBackground = boardBackground;
            _gridBlock = gridBlock;
            _boardGrid = boardGrid;
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

            _board.InitializeCheesePools();
            
            _camera.orthographicSize =
                boardDimensions.BoardHeight * .5f + _gameSettings.Rules.BoardDimensions.BoardPadding;
        }
    }
}