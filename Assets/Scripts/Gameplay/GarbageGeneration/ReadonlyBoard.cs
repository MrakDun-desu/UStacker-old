using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NLua;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class ReadonlyBoard
    {
        private readonly Board _source;
        private readonly bool _isDummy;
        
        public uint Width => _isDummy ? 10u : _source.Width;

        public uint Height => _isDummy ? 20u : _source.Height;
        public uint GarbageHeight => _isDummy ? 5u : _source.GarbageHeight;

        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots =>
            _isDummy ? new List<ReadOnlyCollection<bool>>().AsReadOnly() : _source.Slots;

        public void AddGarbageLayer(List<List<bool>> slots, bool addToLast)
        {
            if (_isDummy) return;
            _source.AddGarbageLayer(slots, addToLast);
        }

        public void AddGarbageLayer(LuaTable slotsTable, bool addToLast)
        {
            var slots = slotsTable.Values.Cast<LuaTable>()
                .Select(entry => entry.Values.Cast<bool>().ToList())
                .Where(line => line.Count == Width && !line.TrueForAll(isOccupied => isOccupied)).ToList();

            _source.AddGarbageLayer(slots, addToLast);
        }

        public ReadonlyBoard(Board source)
        {
            _isDummy = source == null;
            _source = source;
        }

    }
}