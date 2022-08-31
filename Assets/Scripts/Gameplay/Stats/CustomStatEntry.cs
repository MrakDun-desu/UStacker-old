using System.Collections.Generic;

namespace Blockstacker.Gameplay.Stats
{
    public record CustomStatEntry
    {
        public string StatCounterName;
        public Dictionary<string, object> SavedStats;
    }
}