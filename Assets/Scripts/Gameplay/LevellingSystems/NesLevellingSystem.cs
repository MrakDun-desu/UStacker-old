using System.Collections.Generic;
using UnityEngine;

namespace Blockstacker.Gameplay.LevellingSystems
{
    public class NesLevellingSystem : ILevellingSystem
    {
        public LevellingSystemInData InData { get; private set; }
        public LevellingSystemOutData OutData { get; private set; }

        private uint _currentLevel;
        private int _linesToNextLevel;

        // for simplicity - calculation is gravity multiplier divided by frames per row
        // multiplier = nes tetris framerate / reference frame rate = 60.0988 / 60
        // frames per row - taken from https://tetris.wiki/Tetris_(NES,_Nintendo)
        private static readonly double[] _levelGravities = new double[] {
            1.0016466666666666 / 48,
            1.0016466666666666 / 43,
            1.0016466666666666 / 38,
            1.0016466666666666 / 33,
            1.0016466666666666 / 28,
            1.0016466666666666 / 23,
            1.0016466666666666 / 18,
            1.0016466666666666 / 13,
            1.0016466666666666 / 8,
            1.0016466666666666 / 6,
            1.0016466666666666 / 5,
            1.0016466666666666 / 5,
            1.0016466666666666 / 5,
            1.0016466666666666 / 4,
            1.0016466666666666 / 4,
            1.0016466666666666 / 4,
            1.0016466666666666 / 3,
            1.0016466666666666 / 3,
            1.0016466666666666 / 3,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 2,
            1.0016466666666666 / 1
        };

        // used only with the first set level, taken from https://listfist.com/list-of-tetris-levels-by-lines-nes
        // if level is more than 19, 10 is used
        private static readonly int[] _linesToLevelIncrease = new int[] {
            10,
            20,
            30,
            40,
            50,
            60,
            70,
            80,
            90,
            100,
            100,
            100,
            100,
            100,
            100,
            100,
            110,
            120,
            130,
            140
        };

        public void Initialize(LevellingSystemInData inData, LevellingSystemOutData outData, uint startingLevel)
        {
            InData = inData;
            OutData = outData;
            InData.Changed += DataUpdated;
            _currentLevel = (uint)Mathf.Min(startingLevel, 29);
            var linesToNextIndex = (startingLevel > 19) ? 1 : startingLevel;
            _linesToNextLevel = _linesToLevelIncrease[linesToNextIndex];
            OutData.SetValues(CalculateGravity(), 0, _currentLevel, 0);
        }

        private void DataUpdated()
        {
            AddScoreByNewLinesCleared(InData.newLinesCleared);
            AddScoreByNewLinesSoftDropped(InData.newLinesSoftDropped);
        }

        private void AddScoreByNewLinesSoftDropped(int newLinesSoftDropped)
        {
            OutData.SetValues(OutData.gravity, 0, _currentLevel, OutData.score + newLinesSoftDropped);
        }

        private void AddScoreByNewLinesCleared(int newLinesCleared)
        {
            long scoreAddition = newLinesCleared switch
            {
                1 => 40,
                2 => 100,
                3 => 300,
                4 => 1200,
                _ => 0
            };
            scoreAddition *= _currentLevel + 1;
            _linesToNextLevel -= newLinesCleared;
            if (_linesToNextLevel <= 0) {
                _linesToNextLevel += 10;
                _currentLevel += 1;
            }
            OutData.SetValues(CalculateGravity(), 0, _currentLevel, OutData.score + scoreAddition);
        }

        private float CalculateGravity()
        {
            var effectiveLevel = Mathf.Min(29, (int)_currentLevel);
            return (float)_levelGravities[effectiveLevel];
        }

    }
}