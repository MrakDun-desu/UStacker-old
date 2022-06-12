using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Gameplay.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Blockstacker.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _settings;
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private StatCounter _statCounter;
        [SerializeField] private GameRecorder _gameRecorder;

        [Space] [SerializeField] private UnityEvent GameStartedFirstTime;
        [SerializeField] private UnityEvent GameRestartedAfterEnd;
        [SerializeField] private UnityEvent GameStarted;
        [SerializeField] private UnityEvent GamePaused;
        [SerializeField] private UnityEvent GameResumed;
        [SerializeField] private UnityEvent GameRestarted;
        [SerializeField] private UnityEvent GameLost;
        [SerializeField] private UnityEvent GameEnded;
        public GameReplay Replay;
        private bool _gameEnded;
        private bool _gameLost;
        private bool _gameStarted;
        private bool _gameRunning;

        public event Action GameRestartedEvent;
        public event Action GamePausedEvent;
        public event Action GameResumedEvent;
        public event Action GameEndedEvent;

        #region Game event management

        public void StartGame()
        {
            if (!_gameStarted)
                FirstTimeGameStart();
            if (_gameLost || _gameEnded)
                GameRestartAfterEnd();
            _gameRunning = true;
            GameStarted.Invoke();
        }

        private void FirstTimeGameStart()
        {
            _gameStarted = true;
            Replay.GameSettings = _settings.Settings with { };
            GameStartedFirstTime.Invoke();
        }

        private void GameRestartAfterEnd()
        {
            _gameLost = false;
            _gameEnded = false;
            GameRestartedAfterEnd.Invoke();
        }

        public void TogglePause()
        {
            if (_gameEnded || !_gameStarted) return;

            if (_gameRunning)
            {
                GamePaused.Invoke();
                GamePausedEvent?.Invoke();
            }
            else
            {
                GameResumed.Invoke();
                GameResumedEvent?.Invoke();
            }

            _gameRunning = !_gameRunning;
        }

        public void Restart()
        {
            GameRestarted.Invoke();
            GameRestartedEvent?.Invoke();
        }

        public void LoseGame()
        {
            _gameEnded = true;
            if (_settings.Objective.ToppingOutIsOkay)
                EndGame();
            else
            {
                _gameLost = true;
                GameLost.Invoke();
            }
        }

        public void EndGame()
        {
            _gameEnded = true;
            Replay.ActionList = new List<Message>();
            Replay.ActionList.AddRange(_gameRecorder.ActionList);
            Replay.Stats = _statCounter.Stats with { };
            Replay.GameLength = _timer.CurrentTimeAsSpan;
            GameEnded.Invoke();
            GameEndedEvent?.Invoke();
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
        }

        private void Update()
        {
            if (_settings.Objective.GameEndCondition == GameEndCondition.Time &&
                _timer.CurrentTime > _settings.Objective.EndConditionCount)
                EndGame();
        }

        private void OnDestroy()
        {
            _mediator.Clear();
            GameRestartedEvent = null;
            GamePausedEvent = null;
            GameResumedEvent = null;
            GameEndedEvent = null;
        }

        private void OnPiecePlaced(PiecePlacedMessage _)
        {
            switch (_settings.Objective.GameEndCondition)
            {
                case GameEndCondition.Score:
                    break;
                case GameEndCondition.Time:
                    break;
                case GameEndCondition.LinesCleared:
                    if (_statCounter.Stats.LinesCleared >= _settings.Objective.EndConditionCount)
                        EndGame();
                    break;
                case GameEndCondition.CheeseLinesCleared:
                    break;
                case GameEndCondition.PiecesPlaced:
                    if (_statCounter.Stats.PiecesPlaced >= _settings.Objective.EndConditionCount)
                        EndGame();
                    break;
                case GameEndCondition.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}