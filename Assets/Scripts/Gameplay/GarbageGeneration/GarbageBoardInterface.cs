using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using NLua;

namespace UStacker.Gameplay.GarbageGeneration
{
    public class GarbageBoardInterface
    {
        private readonly Board _source;

        public GarbageBoardInterface(Board source)
        {
            _source = source;
        }

        public uint Width => _source.Width;

        [UsedImplicitly]
        public uint Height => _source.Height;
        [UsedImplicitly]
        public uint GarbageHeight => _source.GarbageHeight;
        [UsedImplicitly]
        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots => _source.Slots;

        public void AddGarbageLayer(List<List<bool>> slots, bool addToLast)
        {
            _source.AddGarbageLayer(slots, addToLast);
        }

        [UsedImplicitly]
        public void AddGarbageLayer(LuaTable slotsTable, bool addToLast)
        {
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