using System;
using System.Collections.Generic;
using Blockstacker.Common.Enums;

namespace Blockstacker.GameSettings
{
    public class RotationSystem
    {
        public readonly Dictionary<PieceType, KickTable> KickTables = new()
        {
            {PieceType.IPiece, new KickTable()},
            {PieceType.TPiece, new KickTable()},
            {PieceType.OPiece, new KickTable()},
            {PieceType.JPiece, new KickTable()},
            {PieceType.LPiece, new KickTable()},
            {PieceType.SPiece, new KickTable()},
            {PieceType.ZPiece, new KickTable()}
        };
        
        public KickTable GetKickTable(PieceType pieceType)
        {
            return KickTables.ContainsKey(pieceType) ? KickTables[pieceType] : new KickTable();
        }
    }
}