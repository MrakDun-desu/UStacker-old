using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsBoardDimensions
    {
        public uint BoardHeight = 22;
        public uint BoardWidth = 10;
        public uint LethalHeight = 20;
        public uint PieceSpawnHeight = 21;
        public TopoutCondition TopoutCondition = TopoutCondition.AllBlocksAboveLethal;
        public bool AllowClutchClears = true;
        public uint BoardPadding = 4;
    }
}