using System;
using System.Collections.Generic;

namespace Blockstacker.GameSettings
{
    [Serializable]
    public class RotationSystem
    {
        public Dictionary<string, KickTable> KickTables = new();
        public KickTable DefaultTable = new();

        public KickTable GetKickTable(string pieceType)
        {
            return KickTables.ContainsKey(pieceType) ? KickTables[pieceType] : DefaultTable;
        }
    }
}