using System.Collections.Generic;

namespace Blockstacker.GameSettings
{
    public class RotationSystem
    {
        public readonly Dictionary<string, KickTable> StringKickTables = new()
        {
            {"IPiece", new KickTable()},
            {"TPiece", new KickTable()},
            {"OPiece", new KickTable()},
            {"JPiece", new KickTable()},
            {"LPiece", new KickTable()},
            {"SPiece", new KickTable()},
            {"ZPiece", new KickTable()},
        };

        public KickTable DefaultTable = new();

        public KickTable GetKickTable(string pieceType)
        {
            return StringKickTables.ContainsKey(pieceType) ? StringKickTables[pieceType] : DefaultTable;
        }
        
        
    }
}