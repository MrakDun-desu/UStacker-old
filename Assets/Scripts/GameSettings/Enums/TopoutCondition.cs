using UnityEngine;

namespace Blockstacker.GameSettings.Enums
{
    public enum TopoutCondition : byte
    {
        [Tooltip("Player will topout when piece is unable to spawn")]
        PieceSpawn = 0,

        [Tooltip("Player will topout when they place at least one block of a piece above the lethal height")]
        OneBlockAboveLethal = 1,

        [Tooltip("Player will topout when they place all blocks above the lethal height")]
        AllBlocksAboveLethal = 2
    }
}