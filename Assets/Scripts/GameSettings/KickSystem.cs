using System;

namespace Blockstacker.GameSettings
{
    [Serializable]
    public class KickSystem
    {
        public KickTable IKickTable = new();
        public KickTable TKickTable = new();
        public KickTable OKickTable = new();
        public KickTable JKickTable = new();
        public KickTable LKickTable = new();
        public KickTable SKickTable = new();
        public KickTable ZKickTable = new();
    }
}