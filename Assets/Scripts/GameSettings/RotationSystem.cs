
/************************************
RotationSystem.cs -- created by Marek Dančo (xdanco00)
*************************************/
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
            if (KickTables.ContainsKey(pieceType))
                return KickTables[pieceType];

            if (pieceType.StartsWith("giant"))
                pieceType = pieceType[^1].ToString().ToLowerInvariant();

            return KickTables.ContainsKey(pieceType) ? KickTables[pieceType] : DefaultTable;
        }
    }
}
/************************************
end RotationSystem.cs
*************************************/
