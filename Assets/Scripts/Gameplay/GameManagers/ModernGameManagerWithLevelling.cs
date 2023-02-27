using System;
using UStacker.Gameplay.Communication;
using UnityEngine;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.GameManagers
{
    public class ModernGameManagerWithLevelling : MonoBehaviour, IGameManager
    {
        private const int LINES_PER_LEVEL_INCREASE = 5;
        private const double LEVEL_DROPTIME_MULTIPLIER = 1.5d;
        private const double LEVEL_1_DROPTIME = 1;

        private const uint MIN_LEVEL = 1;
        private const uint MAX_LEVEL = 20;

        private const string LEVEUP_CONDITION_NAME = "Lines";

        private uint _currentLevel;
        private long _currentScore;
        private int _linesToNextLevel;

        private Mediator _mediator;
        private uint _startingLevel;
        private int _totalLinesToNextLevel;

        private int _linesClearedThisLevel => _totalLinesToNextLevel - _linesToNextLevel;

        public void Initialize(string startingLevel, Mediator mediator)
        {
            uint.TryParse(startingLevel, out _startingLevel);
            _mediator = mediator;

            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);
            _mediator.Register<GameStateChangedMessage>(HandleGameStateChange);
        }

        private void ResetState()
        {
            _currentLevel = Math.Clamp(_startingLevel, MIN_LEVEL, MAX_LEVEL);
            _currentScore = 0;
            _linesToNextLevel = 0;
            CalculateLinesToNext();

            _mediator.Send(new ScoreChangedMessage(0, 0));
            _mediator.Send(new GravityChangedMessage(CalculateGravity(), 0));
            _mediator.Send(new LevelChangedMessage(_currentLevel.ToString(), 0));
            _mediator.Send(new LevelUpConditionChangedMessage(0, _totalLinesToNextLevel, _linesClearedThisLevel,
                LEVEUP_CONDITION_NAME));
        }

        private void HandleGameStateChange(GameStateChangedMessage message)
        {
            if (message.NewState != GameState.Initializing)
                return;
            
            ResetState();
        }

        private void HandlePiecePlaced(PiecePlacedMessage message)
        {
            long scoreAddition;
            if (message.WasSpin)
                scoreAddition = ((int) message.LinesCleared + 1) * 400;
            else if (message.WasSpinMini)
            {
                scoreAddition = message.LinesCleared switch
                {
                    0 => 100,
                    1 => 200,
                    2 => 400,
                    var amount => (int) amount * 300
                };
            }
            else
            {
                scoreAddition = message.LinesCleared switch
                {
                    0 => 0,
                    1 => 100,
                    2 => 300,
                    3 => 500,
                    4 => 800,
                    var amount => (int) amount * 200
                };
            }

            scoreAddition += (int) message.CurrentCombo * 50;

            if (scoreAddition == 0)
                return;

            const float backToBackMultiplier = 1.5f;
            if (message.CurrentBackToBack >= 1 && message.LinesCleared > 0)
                scoreAddition = (int) (scoreAddition * backToBackMultiplier);

            if (message.WasAllClear)
                scoreAddition += 3000;

            scoreAddition *= (int) _currentLevel;
            _currentScore += scoreAddition;
            _mediator.Send(new ScoreChangedMessage(_currentScore, message.Time));

            if (_currentLevel > MAX_LEVEL) return;

            _linesToNextLevel -= (int) message.LinesCleared;

            if (_linesToNextLevel <= 0)
            {
                while (_linesToNextLevel <= 0)
                {
                    _currentLevel++;
                    CalculateLinesToNext();
                }

                _mediator.Send(new LevelUpConditionChangedMessage(message.Time, _totalLinesToNextLevel,
                    _linesClearedThisLevel, LEVEUP_CONDITION_NAME));
                _mediator.Send(new LevelChangedMessage(_currentLevel.ToString(), message.Time));
                _mediator.Send(new GravityChangedMessage(CalculateGravity(), message.Time));
            }
            else if (message.LinesCleared > 0)
            {
                _mediator.Send(new LevelUpConditionChangedMessage(message.Time, _totalLinesToNextLevel,
                    _linesClearedThisLevel, LEVEUP_CONDITION_NAME));
            }
        }

        private void HandlePieceMoved(PieceMovedMessage message)
        {
            var scoreAddition = message switch
            {
                {WasHardDrop: true, Y: var y} => -2 * y,
                {WasSoftDrop: true, Y: var y} => -y,
                _ => 0
            };

            if (scoreAddition == 0) return;
            _currentScore += scoreAddition;
            _mediator.Send(new ScoreChangedMessage(_currentScore, message.Time));
        }

        private double CalculateGravity()
        {
            var droptimeMultiplier = Math.Pow(LEVEL_DROPTIME_MULTIPLIER, _currentLevel - 1);
            return 1d / 60d / LEVEL_1_DROPTIME * droptimeMultiplier;
        }

        private void CalculateLinesToNext()
        {
            _totalLinesToNextLevel = (int) _currentLevel * LINES_PER_LEVEL_INCREASE;
            _linesToNextLevel += _totalLinesToNextLevel;
        }
    }
}