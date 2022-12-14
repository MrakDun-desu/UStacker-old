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
            GRAVITY_MULTIPLIER / 48d, // level 0
            GRAVITY_MULTIPLIER / 43d, // level 1
            GRAVITY_MULTIPLIER / 38d, // level 2
            GRAVITY_MULTIPLIER / 33d, // level 3
            GRAVITY_MULTIPLIER / 28d, // level 4
            GRAVITY_MULTIPLIER / 23d, // level 5
            GRAVITY_MULTIPLIER / 18d, // level 6
            GRAVITY_MULTIPLIER / 13d, // level 7
            GRAVITY_MULTIPLIER / 8d,  // level 8
            GRAVITY_MULTIPLIER / 6d,  // level 9
            GRAVITY_MULTIPLIER / 5d,  // level 10
            GRAVITY_MULTIPLIER / 5d,  // level 11
            GRAVITY_MULTIPLIER / 5d,  // level 12
            GRAVITY_MULTIPLIER / 4d,  // level 13
            GRAVITY_MULTIPLIER / 4d,  // level 14
            GRAVITY_MULTIPLIER / 4d,  // level 15
            GRAVITY_MULTIPLIER / 3d,  // level 16
            GRAVITY_MULTIPLIER / 3d,  // level 17
            GRAVITY_MULTIPLIER / 3d,  // level 18
            GRAVITY_MULTIPLIER / 2d,  // level 19
            GRAVITY_MULTIPLIER / 2d,  // level 20
            GRAVITY_MULTIPLIER / 2d,  // level 21
            GRAVITY_MULTIPLIER / 2d,  // level 22
            GRAVITY_MULTIPLIER / 2d,  // level 23
            GRAVITY_MULTIPLIER / 2d,  // level 24
            GRAVITY_MULTIPLIER / 2d,  // level 25
            GRAVITY_MULTIPLIER / 2d,  // level 26
            GRAVITY_MULTIPLIER / 2d,  // level 27
            GRAVITY_MULTIPLIER / 2d,  // level 28
            GRAVITY_MULTIPLIER / 1d   // level 29
        };

        // used only with the first set level, taken from https://listfist.com/list-of-tetris-levels-by-lines-nes
        private static readonly int[] _linesToLevelIncrease =
        {
            10,  // level 0 start
            20,  // level 1 start
            30,  // level 2 start
            40,  // level 3 start
            50,  // level 4 start
            60,  // level 5 start
            70,  // level 6 start
            80,  // level 7 start
            90,  // level 8 start
            100, // level 9 start
            100, // level 10 start
            100, // level 11 start
            100, // level 12 start
            100, // level 13 start
            100, // level 14 start
            100, // level 15 start
            110, // level 16 start
            120, // level 17 start
            130, // level 18 start
            140  // level 19 start
        };

        private MediatorSO _mediator;
        private uint _currentLevel;
        private int _linesToNextLevel;
        private int _totalLinesToNextLevel;
        private int _linesClearedThisLevel => _totalLinesToNextLevel - _linesToNextLevel;
        private long _currentScore;
        private uint _startingLevel;

        private const string LEVELUP_CONDITION_NAME = "Lines";

        public void Initialize(string startingLevel, MediatorSO mediator)
        {
            uint.TryParse(startingLevel, out _startingLevel);
            _mediator = mediator;
            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);
            _mediator.Register<GameStartedMessage>(_ => ResetGameManager());
            _mediator.Register<GameRestartedMessage>(_ => ResetGameManager());
        }

        private void ResetGameManager()
        {
            _currentLevel = (uint) Mathf.Min(_startingLevel, _levelGravities.Length - 1);
            var linesToNextIndex = _currentLevel > _linesToLevelIncrease.Length - 1 ? 0 : _currentLevel;
            _totalLinesToNextLevel = _linesToLevelIncrease[linesToNextIndex];
            _linesToNextLevel = _totalLinesToNextLevel;

            _currentScore = 0;
            _mediator.Send(new ScoreChangedMessage(0, 0));
            _mediator.Send(new GravityChangedMessage(CalculateGravity(), 0));
            _mediator.Send(new LevelChangedMessage(_currentLevel.ToString(), 0));
            _mediator.Send(new LockDelayChangedMessage(0, 0));
            _mediator.Send(new LevelUpConditionChangedMessage(0, _totalLinesToNextLevel, _linesClearedThisLevel, LEVELUP_CONDITION_NAME));
        }

        private void HandlePiecePlaced(PiecePlacedMessage message)
        {
            var scoreAddition = message.LinesCleared switch
            {
                1 => 40,
                2 => 100,
                3 => 300,
                4 => 1200,
                _ => 0
            } * (_currentLevel + 1);
            _currentScore += scoreAddition;
            _mediator.Send(new ScoreChangedMessage(_currentScore, message.Time));
            _linesToNextLevel -= (int)message.LinesCleared;

            if (_linesToNextLevel <= 0)
            {
                while (_linesToNextLevel <= 0)
                {
                    _linesToNextLevel += 10;
                    _totalLinesToNextLevel = 10;
                    _currentLevel++;
                }

                _mediator.Send(new LevelUpConditionChangedMessage(message.Time, _totalLinesToNextLevel,
                    _linesClearedThisLevel, LEVELUP_CONDITION_NAME));
                _mediator.Send(new LevelChangedMessage(_currentLevel.ToString(), message.Time));
                _mediator.Send(new GravityChangedMessage(CalculateGravity(), message.Time));
            }
            else if (message.LinesCleared > 0)
            {
                _mediator.Send(new LevelUpConditionChangedMessage(message.Time, _totalLinesToNextLevel, _linesClearedThisLevel, LEVELUP_CONDITION_NAME));
            }
        }

        private void HandlePieceMoved(PieceMovedMessage pieceMoved)
        {
            if (!pieceMoved.WasSoftDrop) return;

            _currentScore += -pieceMoved.Y;
            _mediator.Send(new ScoreChangedMessage(_currentScore, pieceMoved.Time));
        }

        private double CalculateGravity()
        {
            var effectiveLevel = Mathf.Min(_levelGravities.Length - 1, (int) _currentLevel);
            return _levelGravities[effectiveLevel];
        }
    }
}