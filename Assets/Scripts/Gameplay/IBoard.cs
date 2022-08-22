using System.Collections.Generic;
using System.Collections.ObjectModel;
using NLua;

namespace Blockstacker.Gameplay
{
    public interface IBoard
    {
        uint Width { get; }
        uint Height { get; }
        uint GarbageHeight { get; }
        
        ReadOnlyCollection<ReadOnlyCollection<bool>> Slots { get; }

        void AddCheeseLayer(List<List<bool>> slots, bool addToLast);
        void AddCheeseLayer(LuaTable slotsTable, bool addToLast);
    }
}