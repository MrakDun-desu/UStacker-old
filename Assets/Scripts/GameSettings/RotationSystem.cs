using System;
using System.Collections.Generic;

namespace UStacker.GameSettings
{
    [Serializable]
    public class RotationSystem
    {
        public KickTable DefaultTable = new();
        public Dictionary<string, KickTable> KickTables = new();

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