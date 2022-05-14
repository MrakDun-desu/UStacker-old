using System;

namespace Gameplay.Stats
{
    [Serializable]
    public class StatContainer
    {
        public uint LinesCleared;
        public uint PiecesPlaced;
        public uint KeysPressed;
        public uint Singles;
        public uint Doubles;
        public uint Triples;
        public uint Quads;
    }
}