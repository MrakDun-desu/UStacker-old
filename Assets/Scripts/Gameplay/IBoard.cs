using System.Collections.Generic;
using System.Collections.ObjectModel;
using NLua;

namespace Blockstacker.Gameplay
{
    public interface IBoard
    {
        uint Width { get; }
        uint Height { get; }
        uint CheeseHeight { get; }
        
        ReadOnlyCollection<ReadOnlyCollection<bool>> Slots { get; }

        void AddCheeseLines(IEnumerable<IEnumerable<bool>> slots);
        void AddCheeseLines(LuaTable slotsTable);
    }
}