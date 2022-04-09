using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.LevellingSystems
{
    public class LevellingSystemInData
    {
        public float timePassed;
        public int newLinesCleared;
        public int newCheeseLinesCleared;
        public int newLinesSoftDropped;
        public int newLinesHardDropped;
        public int newFinesseFaults;
        public bool wasPiecePlaced;
        public bool wasSpin;
        public bool wasSpinMini;
        public bool wasAllClear;
        public bool wasColorClear;

        public Dictionary<string, object> customData;

        public void Confirm() => Changed?.Invoke();

        public event Action Changed;
    }
}