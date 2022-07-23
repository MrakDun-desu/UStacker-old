using System.Collections.Generic;

namespace Blockstacker.GameSettings
{
    public class RotationSystem
    {
        public readonly Dictionary<string, KickTable> KickTables = new()
        {
            {"t", new KickTable()},
            {"i", new KickTable()},
            {"o", new KickTable()},
            {"l", new KickTable()},
            {"j", new KickTable()},
            {"s", new KickTable()},
            {"z", new KickTable()},
        };

        public KickTable DefaultTable = new();

        public KickTable GetKickTable(string pieceType)
        {
            return KickTables.ContainsKey(pieceType) ? KickTables[pieceType] : DefaultTable;
        }
        
        
    }
}