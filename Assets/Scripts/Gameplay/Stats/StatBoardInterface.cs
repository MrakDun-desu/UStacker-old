using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Blockstacker.Gameplay.Stats
{
    public class StatBoardInterface
    {
        private readonly Board _source;
        
        [UsedImplicitly]
        public uint Width => _source.Width;
        [UsedImplicitly]
        public uint Height => _source.Height;
        [UsedImplicitly]
        public uint GarbageHeight => _source.GarbageHeight;

        [UsedImplicitly]
        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots => _source.Slots;

        public StatBoardInterface(Board source)
        {
            _source = source;
        }
    }
}