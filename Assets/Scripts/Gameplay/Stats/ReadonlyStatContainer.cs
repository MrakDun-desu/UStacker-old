using System;

namespace Gameplay.Stats
{
    [Serializable]
    public record ReadonlyStatContainer
    {
        private StatContainer _parent;

        public uint LinesCleared => _parent.LinesCleared;
        public uint PiecesPlaced => _parent.PiecesPlaced;
        public uint KeysPressed => _parent.KeysPressed;
        public uint Singles => _parent.Singles;
        public uint Doubles => _parent.Doubles;
        public uint Triples => _parent.Triples;
        public uint Quads => _parent.Quads;
        public uint Spins => _parent.Spins;
        public uint MiniSpins => _parent.MiniSpins;
        public uint SpinSingles => _parent.SpinSingles;
        public uint SpinDoubles => _parent.SpinDoubles;
        public uint SpinTriples => _parent.SpinTriples;
        public uint SpinQuads => _parent.SpinQuads;
        public uint MiniSpinSingles => _parent.MiniSpinSingles;
        public uint MiniSpinDoubles => _parent.MiniSpinDoubles;
        public uint MiniSpinTriples => _parent.MiniSpinTriples;
        public uint MiniSpinQuads => _parent.MiniSpinQuads;
        public uint LongestCombo => _parent.LongestCombo;
        public uint LongestBackToBack => _parent.LongestBackToBack;
        public uint AllClears => _parent.AllClears;
        public uint Holds => _parent.Holds;
        public uint GarbageLinesCleared => _parent.GarbageLinesCleared;
        public double PiecesPerSecond => _parent.PiecesPerSecond;
        public double KeysPerPiece => _parent.KeysPerPiece;
        public double KeysPerSecond => _parent.KeysPerSecond;
        public double LinesPerMinute => _parent.LinesPerMinute;
        
        public ReadonlyStatContainer(StatContainer parent)
        {
            _parent = parent;
        }

        public static implicit operator StatContainer(ReadonlyStatContainer source)
        {
            return source._parent with {};
        }
        
        public static implicit operator ReadonlyStatContainer(StatContainer source)
        {
            return new ReadonlyStatContainer(source);
        }
    }
}