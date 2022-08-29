using Blockstacker.Gameplay.Communication;
using UnityEngine;

namespace Blockstacker.Gameplay.Levelling
{
    public class NesLevellingSystem
    {
        // for simplicity - calculation is gravity multiplier divided by frames per row
        // multiplier = nes tetris framerate / reference frame rate = 60.0988 / 60
        // frames per row - taken from https://tetris.wiki/Tetris_(NES,_Nintendo)
        private static readonly double[] _levelGravities =
        {
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
        private static readonly uint[] _linesToLevelIncrease =
        {
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

        private readonly MediatorSO _mediator;
        private uint _currentLevel;
        private uint _linesToNextLevel;

        public NesLevellingSystem(uint startingLevel, MediatorSO mediator)
        {
            _currentLevel = (uint) Mathf.Min(startingLevel, 29);
            var linesToNextIndex = startingLevel > 19 ? 1 : startingLevel;
            _linesToNextLevel = _linesToLevelIncrease[linesToNextIndex];
            _mediator = mediator;

            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);

            var newGravity = new GravityChangedMessage {Gravity = CalculateGravity()};
            var newLevel = new LevelChangedMessage {Level = _currentLevel};
            var newLockDelay = new LockDelayChangedMessage {LockDelay = 0};
            _mediator.Send(newGravity);
            _mediator.Send(newLevel);
            _mediator.Send(newLockDelay);
        }

        private void HandlePiecePlaced(PiecePlacedMessage piecePlaced)
        {
            long scoreAddition = piecePlaced.LinesCleared switch
            {
                1 => 40,
                2 => 100,
                3 => 300,
                4 => 1200,
                _ => 0
            };
            scoreAddition *= _currentLevel + 1;
            _linesToNextLevel -= piecePlaced.LinesCleared;
            if (_linesToNextLevel <= 0)
            {
                _linesToNextLevel += 10;
                _currentLevel += 1;

                var newGravity = new GravityChangedMessage {Gravity = CalculateGravity(), Time = piecePlaced.Time};
                var newLevel = new LevelChangedMessage {Level = _currentLevel, Time = piecePlaced.Time};
                _mediator.Send(newGravity);
                _mediator.Send(newLevel);
            }

            var newScore = new ScoreAddedMessage {Score = scoreAddition, Time = piecePlaced.Time};
            _mediator.Send(newScore);
        }

        private void HandlePieceMoved(PieceMovedMessage pieceMoved)
        {
            if (!pieceMoved.WasSoftDrop) return;

            var newScore = new ScoreAddedMessage {Score = -(long) pieceMoved.Y, Time = pieceMoved.Time};
            _mediator.Send(newScore);
        }

        private double CalculateGravity()
        {
            var effectiveLevel = Mathf.Min(_levelGravities.Length - 1, (int) _currentLevel);
            return _levelGravities[effectiveLevel];
        }
    }
}