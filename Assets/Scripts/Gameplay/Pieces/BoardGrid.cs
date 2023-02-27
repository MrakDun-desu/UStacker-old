using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Blocks;
using UnityEngine;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;
using UStacker.GlobalSettings;

namespace UStacker.Gameplay.Pieces
{
    public class BoardGrid : MonoBehaviour, IBlockCollection, IGameSettingsDependency
    {
        [SerializeField] private Board _board;
        [SerializeField] private BlockBase _gridBlock;
        
        private readonly List<BlockBase> _blocks = new();
        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);
        public string Type => "grid";
        public GameSettingsSO.SettingsContainer GameSettings { private get; set; }

        private void Awake()
        {
            for (var y = 0; y < GameSettings.BoardDimensions.BoardHeight; y++)
            for (var x = 0; x < GameSettings.BoardDimensions.BoardWidth; x++)
            {
                var gridBlock = Instantiate(
                    _gridBlock,
                    transform,
                    false
                );

                gridBlock.Visibility = AppSettings.Gameplay.GridVisibility;

                var blockTransform = gridBlock.transform;
                blockTransform.localPosition = new Vector3(x + .5f, y + .5f, blockTransform.localPosition.z);

                _blocks.Add(gridBlock);
                gridBlock.Board = _board;
            }
        }
    }
}