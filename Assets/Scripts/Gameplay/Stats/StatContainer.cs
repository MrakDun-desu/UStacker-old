using System;

namespace Gameplay.Stats
{
    [Serializable]
    public record StatContainer
    {
        public uint LinesCleared;
        public uint PiecesPlaced;
        public uint KeysPressed;
        public uint Singles;
        public uint Doubles;
        public uint Triples;
        public uint Quads;
        public uint Spins;
        public uint MiniSpins;
        public uint SpinSingles;
        public uint SpinDoubles;
        public uint SpinTriples;
        public uint SpinQuads;
        public uint MiniSpinSingles;
        public uint MiniSpinDoubles;
        public uint MiniSpinTriples;
        public uint MiniSpinQuads;

        public uint LongestCombo;
        public uint LongestBackToBack;
        public uint AllClears;
        public uint Holds;

        public void Reset()
        {
            foreach (var fieldInfo in GetType().GetFields())
            {
                fieldInfo.SetValue(this, default);
            }
        }
    }
}