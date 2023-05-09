
/************************************
StatBoardInterface.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace UStacker.Gameplay.Stats
{
    public class StatBoardInterface
    {
        private readonly Board _source;

        public StatBoardInterface(Board source)
        {
            _source = source;
        }

        [UsedImplicitly] public uint Width => _source.Width;
        [UsedImplicitly] public uint Height => _source.Height;
        [UsedImplicitly] public uint GarbageHeight => _source.GarbageHeight;

        [UsedImplicitly] public uint LethalHeight => _source.LethalHeight;

        [UsedImplicitly] public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots => _source.Slots;
    }
}
/************************************
end StatBoardInterface.cs
*************************************/
