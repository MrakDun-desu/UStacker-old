using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.LevellingSystems
{
    public class GuidelineLevellingSystem : ILevellingSystem
    {
        public LevellingSystemInData InData { get; private set; }
        public LevellingSystemOutData OutData { get; private set; }

        private uint _currentLevel;
        private int _linesToNextLevel;
        private bool _isBackToBack;
        private int _currentCombo;

        public void Initialize(LevellingSystemInData inData, LevellingSystemOutData outData, uint startingLevel)
        {
            InData = inData;
            OutData = outData;
            InData.Changed += DataUpdated;
            OutData.SetValues(CalculateGravity(), CalculateLockDelay(), _currentLevel, 0);
        }

        private void DataUpdated()
        {

        }

        private float CalculateGravity()
        {
            return .2f;
        }

        private float CalculateLockDelay()
        {
            return .5f;
        }
    }
}