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

        public void Reset()
        {
            LinesCleared = 0;
            PiecesPlaced = 0;
            KeysPressed = 0;
            Singles = 0;
            Doubles = 0;
            Triples = 0;
            Quads = 0;
        }
    }
}