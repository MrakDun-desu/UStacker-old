
/************************************
EndConditionChecker.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Stats;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;

namespace UStacker.Gameplay.GameStateManagement
{
    public class EndConditionChecker : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private GameTimer _timer;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private StatCounterManager _statCounterManager;
        public UnityEvent<double> GameEnded;

        private GameState _gameState;
        private double _lastSentCondition;

        private void Update()
        {
            if (GameSettings.Objective.GameEndCondition != GameEndCondition.Time ||
                _gameState is not GameState.Running) return;

            var functionStartTime = _timer.CurrentTime;

            double currentTimeRounded = (int) functionStartTime;
            if (_lastSentCondition < currentTimeRounded)
            {
                _lastSentCondition = currentTimeRounded;
                _mediator.Send(new GameEndConditionChangedMessage(
                    functionStartTime,
                    GameSettings.Objective.EndConditionCount,
                    currentTimeRounded,
                    GameSettings.Objective.GameEndCondition.ToString()));
            }

            if (!(functionStartTime > GameSettings.Objective.EndConditionCount)) return;
            GameEnded.Invoke(GameSettings.Objective.EndConditionCount);
        }

        private void OnEnable()
        {
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced);
            _mediator.Register<ScoreChangedMessage>(OnScoreChanged);
            _mediator.Register<GameStateChangedMessage>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            _mediator.Unregister<PiecePlacedMessage>(OnPiecePlaced);
            _mediator.Unregister<ScoreChangedMessage>(OnScoreChanged);
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChanged);
        }

        public GameSettingsSO.SettingsContainer GameSettings { get; set; }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            _gameState = message.NewState;

            if (_gameState != GameState.Initializing) return;

            _mediator.Send(new GameEndConditionChangedMessage(
                0,
                GameSettings.Objective.EndConditionCount,
                0,
                GameSettings.Objective.GameEndCondition.ToString()));
            _lastSentCondition = 0;
        }

        private void OnScoreChanged(ScoreChangedMessage message)
        {
            if (GameSettings.Objective.GameEndCondition != GameEndCondition.Score) return;

            _mediator.Send(new GameEndConditionChangedMessage(
                message.Time,
                GameSettings.Objective.EndConditionCount,
                message.Score,
                GameSettings.Objective.GameEndCondition.ToString()));

            if (message.Score >= GameSettings.Objective.EndConditionCount)
                GameEnded.Invoke(message.Time);
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            var endCondition = GameSettings.Objective.GameEndCondition;
            var endConditionCount = GameSettings.Objective.EndConditionCount;

            switch (endCondition)
            {
                case GameEndCondition.LinesCleared:
                    var linesCleared = _statCounterManager.Stats.LinesCleared;
                    _mediator.Send(new GameEndConditionChangedMessage(
                        message.Time,
                        endConditionCount,
                        linesCleared,
                        endCondition.ToString()));
                    if (linesCleared >= GameSettings.Objective.EndConditionCount)
                        GameEnded.Invoke(message.Time);
                    break;
                case GameEndCondition.GarbageLinesCleared:
                    var garbageLinesCleared = _statCounterManager.Stats.GarbageLinesCleared;
                    _mediator.Send(new GameEndConditionChangedMessage(
                        message.Time,
                        endConditionCount,
                        garbageLinesCleared,
                        endCondition.ToString()));
                    if (garbageLinesCleared >= GameSettings.Objective.EndConditionCount)
                        GameEnded.Invoke(message.Time);
                    break;
                case GameEndCondition.PiecesPlaced:
                    var piecesPlaced = _statCounterManager.Stats.PiecesPlaced;
                    _mediator.Send(new GameEndConditionChangedMessage(
                        message.Time,
                        endConditionCount,
                        piecesPlaced,
                        endCondition.ToString()));
                    if (piecesPlaced >= GameSettings.Objective.EndConditionCount)
                        GameEnded.Invoke(message.Time);
                    break;
                case GameEndCondition.Time:
                case GameEndCondition.Score:
                case GameEndCondition.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
/************************************
end EndConditionChecker.cs
*************************************/
