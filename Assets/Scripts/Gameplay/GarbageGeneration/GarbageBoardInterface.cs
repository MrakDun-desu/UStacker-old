using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using NLua;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class GarbageBoardInterface
    {
        private readonly bool _isDummy;
        private readonly Board _source;

        public GarbageBoardInterface(Board source)
        {
            _isDummy = source == null;
            _source = source;
        }

        public uint Width => _isDummy ? 10u : _source.Width;

        [UsedImplicitly]
        public uint Height => _isDummy ? 20u : _source.Height;
        [UsedImplicitly]
        public uint GarbageHeight => _isDummy ? 5u : _source.GarbageHeight;
        [UsedImplicitly]
        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots =>
            _isDummy ? new List<ReadOnlyCollection<bool>>().AsReadOnly() : _source.Slots;

        public void AddGarbageLayer(List<List<bool>> slots, bool addToLast)
        {
            if (_isDummy) return;
            _source.AddGarbageLayer(slots, addToLast);
        }

        [UsedImplicitly]
        public void AddGarbageLayer(LuaTable slotsTable, bool addToLast)
        {
            if (_isDummy) return;
            var slots = slotsTable.Values.Cast<LuaTable>()
                .Select(entry => entry.Values.Cast<bool>().ToList())
                .Where(line => line.Count == Width &&
                               !line.TrueForAll(isOccupied => isOccupied) &&
                               !line.TrueForAll(isOccupied => !isOccupied))
                .ToList();

            _source.AddGarbageLayer(slots, addToLast);
        }
    }
}