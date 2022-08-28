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
            if (pieceType.StartsWith("giant"))
            {
                var simplifiedPieceType = pieceType[^1].ToString().ToLowerInvariant();
                return KickTables.ContainsKey(simplifiedPieceType) ? KickTables[simplifiedPieceType] : DefaultTable;
            }

            return KickTables.ContainsKey(pieceType) ? KickTables[pieceType] : DefaultTable;
        }
    }
}