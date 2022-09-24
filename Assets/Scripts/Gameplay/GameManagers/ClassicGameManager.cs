using Blockstacker.Gameplay.Communication;
using UnityEngine;

namespace Blockstacker.Gameplay.GameManagers
{
    public class ClassicGameManager : MonoBehaviour, IGameManager
    {
        // nes tetris framerate / reference frame rate = 60.0988 / 60
        private const double GRAVITY_MULTIPLIER = 1.001646666666666666;
        
        // for simplicity - calculation is gravity multiplier divided by frames per row
        // frames per row - taken from https://tetris.wiki/Tetris_(NES,_Nintendo)
        private static readonly double[] _levelGravities =
        {
            GRAVITY_MULTIPLIER / 48d,
            GRAVITY_MULTIPLIER / 43d,
            GRAVITY_MULTIPLIER / 38d,
            GRAVITY_MULTIPLIER / 33d,
            GRAVITY_MULTIPLIER / 28d,
            GRAVITY_MULTIPLIER / 23d,
            GRAVITY_MULTIPLIER / 18d,
            GRAVITY_MULTIPLIER / 13d,
            GRAVITY_MULTIPLIER / 8d,
            GRAVITY_MULTIPLIER / 6d,
            GRAVITY_MULTIPLIER / 5d,
            GRAVITY_MULTIPLIER / 5d,
            GRAVITY_MULTIPLIER / 5d,
            GRAVITY_MULTIPLIER / 4d,
            GRAVITY_MULTIPLIER / 4d,
            GRAVITY_MULTIPLIER / 4d,
            GRAVITY_MULTIPLIER / 3d,
            GRAVITY_MULTIPLIER / 3d,
            GRAVITY_MULTIPLIER / 3d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 2d,
            GRAVITY_MULTIPLIER / 1d
        };

        // used only with the first set level, taken from https://listfist.com/list-of-tetris-levels-by-lines-nes
        private static readonly uint[] _linesToLevelIncrease =
        {
            10u,
            20u,
            30u,
            40u,
            50u,
            60u,
            70u,
            80u,
            90u,
            100u,
            100u,
            100u,
            100u,
            100u,
            100u,
            100u,
            110u,
            120u,
            130u,
            140u
        };

        private MediatorSO _mediator;
        private uint _currentLevel;
        private uint _linesToNextLevel;
        private long _currentScore;

        public void Initialize(uint startingLevel, MediatorSO mediator)
        {
            _currentLevel = (uint) Mathf.Min(startingLevel, 29);
            var linesToNextIndex = startingLevel > 19 ? 0 : startingLevel;
            _linesToNextLevel = _linesToLevelIncrease[linesToNextIndex];
            _mediator = mediator;

            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);

            var newGravity = new GravityChangedMessage(CalculateGravity(), 0);
            var newLevel = new LevelChangedMessage(_currentLevel, 0);
            var newLockDelay = new LockDelayChangedMessage(0, 0);
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
            _linesToNextLevel -= piecePlaced.LinesCleared;
            _currentScore += scoreAddition;
            _mediator.Send(new ScoreChangedMessage(_currentScore, piecePlaced.Time));

            if (_linesToNextLevel > 0) return;
            _linesToNextLevel += 10;
            _currentLevel += 1;

            var newGravity = new GravityChangedMessage(CalculateGravity(), piecePlaced.Time);
            var newLevel = new LevelChangedMessage(_currentLevel, piecePlaced.Time);
            _mediator.Send(newGravity);
            _mediator.Send(newLevel);
        }

        private void HandlePieceMoved(PieceMovedMessage pieceMoved)
        {
            if (!pieceMoved.WasSoftDrop) return;

            _currentScore += -(long) pieceMoved.Y;
            _mediator.Send(new ScoreChangedMessage(_currentScore, pieceMoved.Time));
        }

        private double CalculateGravity()
        {
            var effectiveLevel = Mathf.Min(_levelGravities.Length - 1, (int) _currentLevel);
            return _levelGravities[effectiveLevel];
        }
    }
}