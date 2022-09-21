using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Blockstacker.Gameplay.Stats
{
    public class StatBoardInterface
    {
        private readonly Board _source;
        private readonly bool _isDummy;
        
        [UsedImplicitly]
        public uint Width => _isDummy ? 10u : _source.Width;
        [UsedImplicitly]
        public uint Height => _isDummy ? 20u : _source.Height;
        [UsedImplicitly]
        public uint GarbageHeight => _isDummy ? 5u : _source.GarbageHeight;

        public ReadOnlyCollection<ReadOnlyCollection<bool>> Slots =>
            _isDummy ? new List<ReadOnlyCollection<bool>>().AsReadOnly() : _source.Slots;

        public StatBoardInterface(Board source)
        {
            _isDummy = source == null;
            _source = source;
        }
    }
}