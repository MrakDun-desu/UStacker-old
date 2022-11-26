using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Initialization;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.Gameplay.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Blockstacker.Gameplay
{
    public class GameStateManager : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private StatCounterManager _statCounterManager;
        [SerializeField] private GameRecorder _gameRecorder;
        [SerializeField] private GameResultDisplayer _resultDisplayer;

        [Space] 
        [SerializeField] private UnityEvent GameStarted;
        [SerializeField] private UnityEvent GamePaused;
        [SerializeField] private UnityEvent GameUnpaused;
        [SerializeField] private UnityEvent GameResumed;
        [SerializeField] private UnityEvent GameRestarted;
        [SerializeField] private UnityEvent GameLost;
        [SerializeField] private UnityEvent GameEnded;
        
        public GameSettingsSO GameSettings { set => _settings = value; }
        private GameSettingsSO _settings;
        
        private readonly GameReplay Replay = new();
        private bool _gameEnded;
        private bool _gameLost;
        private bool _gameStarted;

        private double _lastSentTimeCondition;

        public bool GameRunning { get; private set; }
        private bool _gamePaused;

        #region Game event management

        public void StartGame()
        {
            if (_gamePaused)
            {
                GameResumed.Invoke();
                _mediator.Send(new GameResumedMessage());
                _gamePaused = false;
                return;
            }
            
            if (!_gameStarted)
                FirstTimeGameStart();
            if (_gameLost || _gameEnded)
                GameRestartAfterEnd();
            GameRunning = true;
            _mediator.Send(new GameStartedMessage(_settings.General.ActiveSeed));
            _mediator.Send(new GameEndConditionChangedMessage(
                0,
                _settings.Objective.EndConditionCount,
                0,
                _settings.Objective.GameEndCondition.ToString()));
            GameStarted.Invoke();
        }

        private void FirstTimeGameStart()
        {
            _gameStarted = true;
            Replay.GameSettings = _settings.Settings with { };
        }

        private void GameRestartAfterEnd()
        {
            _gameLost = false;
            _gameEnded = false;
        }

        public void TogglePause()
        {
            if (_gameEnded || !_gameStarted) return;

            if (GameRunning)
            {
                GamePaused.Invoke();
                _mediator.Send(new GamePausedMessage());
                _gamePaused = true;
            }
            else
            {
                GameUnpaused.Invoke();
            }

            GameRunning = !GameRunning;
        }
        
        public void Restart()
        {
            if (!_gamePaused)
            {
                _gamePaused = false;
                GameResumed.Invoke();
            }
            
            GameRestarted.Invoke();
            _mediator.Send(new GameRestartedMessage());
            _mediator.Send(new GameEndConditionChangedMessage(
                0, 
                _settings.Objective.EndConditionCount, 
                0,
                _settings.Objective.GameEndCondition.ToString()));
        }

        public void LoseGame(double loseTime)
        {
            if (_gameEnded) return;
            
            _gameEnded = true;
            if (_settings.Objective.ToppingOutIsOkay)
                EndGame(loseTime);
            else
            {
                _gameLost = true;
                _mediator.Send(new GameLostMessage());
                GameLost.Invoke();
            }
        }

        public void EndGame(double endTime)
        {
            _gameEnded = true;
            Replay.ActionList = new List<InputActionMessage>();
            Replay.ActionList.AddRange(_gameRecorder.ActionList);
            Replay.Stats = _statCounterManager.Stats;
            Replay.GameLength = endTime;
            _resultDisplayer.DisplayedReplay = Replay;
            GameEnded.Invoke();
            _mediator.Send(new GameEndedMessage(endTime));
        }

        public void TogglePause(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                TogglePause();
        }

        public void Restart(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                Restart();
        }

        #endregion

        #region Game end condition checks

        private void Awake()
        {
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced);
            _mediator.Register<ScoreChangedMessage>(OnScoreChanged);
        }

        private void Update()
        {
            if (_settings.Objective.GameEndCondition != GameEndCondition.Time || _gameEnded) return;

            var functionStartTime = _timer.CurrentTime;

            double currentTimeRounded = (int) functionStartTime;
            if (_lastSentTimeCondition < currentTimeRounded)
            {
                _lastSentTimeCondition = currentTimeRounded;
                _mediator.Send(new GameEndConditionChangedMessage(
                    functionStartTime,
                    _settings.Objective.EndConditionCount,
                    currentTimeRounded,
                    _settings.Objective.GameEndCondition.ToString()));
            }

            if (functionStartTime > _settings.Objective.EndConditionCount)
                EndGame(_settings.Objective.EndConditionCount);
        }

        private void OnDestroy()
        {
            _mediator.Clear();
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            var endCondition = _settings.Objective.GameEndCondition;
            var endConditionCount = _settings.Objective.EndConditionCount;
            switch (endCondition)
            {
                case GameEndCondition.Time:
                    break;
                case GameEndCondition.LinesCleared:
                    var linesCleared = _statCounterManager.Stats.LinesCleared;
                    _mediator.Send(new GameEndConditionChangedMessage(
                        message.Time,
                        endConditionCount,
                        linesCleared,
                        endCondition.ToString()));
                    if (linesCleared >= _settings.Objective.EndConditionCount)
                        EndGame(message.Time);
                    break;
                case GameEndCondition.GarbageLinesCleared:
                    var garbageLinesCleared = _statCounterManager.Stats.GarbageLinesCleared;
                    _mediator.Send(new GameEndConditionChangedMessage(
                        message.Time,
                        endConditionCount,
                        garbageLinesCleared,
                        endCondition.ToString()));
                    if (garbageLinesCleared >= _settings.Objective.EndConditionCount)
                        EndGame(message.Time);
                    break;
                case GameEndCondition.PiecesPlaced:
                    var piecesPlaced = _statCounterManager.Stats.PiecesPlaced;
                    _mediator.Send(new GameEndConditionChangedMessage(
                        message.Time,
                        endConditionCount,
                        piecesPlaced,
                        endCondition.ToString()));
                    if (piecesPlaced >= _settings.Objective.EndConditionCount)
                        EndGame(message.Time);
                    break;
                case GameEndCondition.Score:
                case GameEndCondition.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnScoreChanged(ScoreChangedMessage message)
        {
            if (_settings.Objective.GameEndCondition != GameEndCondition.Score) return;

            _mediator.Send(new GameEndConditionChangedMessage(
                message.Time,
                _settings.Objective.EndConditionCount,
                message.Score,
                _settings.Objective.GameEndCondition.ToString()));

            if (message.Score >= _settings.Objective.EndConditionCount)
                EndGame(message.Time);
        }

        #endregion
    }
}