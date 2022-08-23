using System.Collections.Generic;
using System.Collections.ObjectModel;
using NLua;

namespace Blockstacker.Gameplay
{
    public class ReadonlyBoard
    {
        private readonly Board _source;

        public uint Width => _source.Width;
        public uint Height => _source.Height;
        public uint GarbageHeight => _source.GarbageHeight;
        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots => _source.Slots;

        public void AddGarbageLayer(IList<List<bool>> slots, bool addToLast) =>
            _source.AddGarbageLayer(slots, addToLast);

        public void AddGarbageLayer(LuaTable slots, bool addToLast) =>
            _source.AddGarbageLayer(slots, addToLast);
        
        public ReadonlyBoard(Board source)
        {
            _source = source;
        }
    }
}