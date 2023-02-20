using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Stats;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using UStacker.GlobalSettings;

namespace UStacker.Gameplay
{
    public class GameStateManager : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private GameTimer _timer;
        [SerializeField] private StatCounterManager _statCounterManager;
        [SerializeField] private GameRecorder _gameRecorder;
        [SerializeField] private GameResultDisplayer _resultDisplayer;
        [SerializeField] private Mediator _mediator;

        [Space]
        [SerializeField] private UnityEvent GameStarted;
        [SerializeField] private UnityEvent GamePaused;
        [SerializeField] private UnityEvent GameUnpaused;
        [SerializeField] private UnityEvent GameResumed;
        [SerializeField] private UnityEvent GameRestarted;
        [SerializeField] private UnityEvent GameLost;
        [SerializeField] private UnityEvent GameEnded;
        [SerializeField] private UnityEvent ReplayStarted;
        [SerializeField] private UnityEvent ReplayFinished;
        [SerializeField] public UnityEvent ReplayPaused;
        [SerializeField] public UnityEvent ReplayUnpaused;

        private readonly GameReplay Replay = new();
        private bool _gameCurrentlyInProgress;
        private bool _gameEnded;
        private bool _gamePauseable = true;
        private bool _gamePaused;

        private double _lastSentTimeCondition;
        private GameSettingsSO.SettingsContainer _settings;

        public string GameType { get; set; }
        public bool IsReplaying { get; set; }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set => _settings = value;
        }

        #region Game event management

        public void StartGame()
        {
            if (_gamePaused)
            {
                GameResumed.Invoke();
                _mediator.Send(new GameResumedMessage());
                _gamePaused = false;
            }

            if (_gameCurrentlyInProgress) return;

            _gameCurrentlyInProgress = true;

            _gameEnded = false;

            _lastSentTimeCondition = 0;
            _mediator.Send(new GameStartedMessage(_settings.General.ActiveSeed));
            _mediator.Send(new GameEndConditionChangedMessage(
                0,
                _settings.Objective.EndConditionCount,
                0,
                _settings.Objective.GameEndCondition.ToString()));
            GameStarted.Invoke();
            if (IsReplaying)
                ReplayStarted.Invoke();
        }

        public void TogglePause()
        {
            if (_gameEnded) return;

            if (_gamePauseable)
            {
                GamePaused.Invoke();
                if (IsReplaying)
                    ReplayPaused.Invoke();
                _mediator.Send(new GamePausedMessage());
                _gamePaused = true;
                _gamePauseable = false;
                return;
            }
            _gamePauseable = true;
            if (IsReplaying)
            {
                ReplayUnpaused.Invoke();
                _gamePaused = false;
            }
            else
                GameUnpaused.Invoke();

        }

        public void Restart()
        {
            _gameEnded = false;
            if (_gamePaused)
            {
                _gamePauseable = true;
                _gamePaused = false;
                GameResumed.Invoke();
            }

            _gameCurrentlyInProgress = false;
            _gamePauseable = true;
            _timer.ResetTiming();
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
                _gameCurrentlyInProgress = false;
                _mediator.Send(new GameLostMessage());
                GameLost.Invoke();
            }
        }

        public void EndGame(double endTime)
        {
            _gameEnded = true;
            _gameCurrentlyInProgress = false;
            if (!IsReplaying)
            {
                Replay.GameType = GameType;
                Replay.GameSettings = _settings with {};
                Replay.ActionList.Clear();
                _gameRecorder.ActionList.Sort((a,b) => a.Time.CompareTo(b.Time));
                Replay.ActionList.AddRange(_gameRecorder.ActionList);
                Replay.PiecePlacementList.Clear();
                Replay.PiecePlacementList.AddRange(_gameRecorder.PiecePlacementList);
                Replay.Stats = _statCounterManager.Stats;
                Replay.GameLength = endTime;
                Replay.TimeStamp = DateTime.UtcNow;
                if (AppSettings.Gameplay.AutosaveReplaysOnDisk)
                    Replay.Save();
                
                _resultDisplayer.DisplayedReplay = Replay;
            }
            else
                ReplayFinished.Invoke();

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
            if (!ctx.performed) return;
            Restart();
        }

        #endregion

        #region Game end condition checks

        private void Awake()
        {
            AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocusChanged += OnPauseSettingChange;
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced);
            _mediator.Register<ScoreChangedMessage>(OnScoreChanged);
            OnPauseSettingChange(AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocus);
        }

        private void OnPauseSettingChange(bool pauseGameOnFocusLoss)
        {
            if (pauseGameOnFocusLoss)
                Application.focusChanged += OnFocusChanged;
            else
                Application.focusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(bool hasFocus)
        {
            if (!hasFocus && _gamePauseable)
                TogglePause();
        }

        private void Update()
        {
            if (_settings.Objective.GameEndCondition != GameEndCondition.Time || _gameEnded) return;

            var functionStartTime = _timer.CurrentTime;

            double currentTimeRounded = (int)functionStartTime;
            if (_lastSentTimeCondition < currentTimeRounded)
            {
                _lastSentTimeCondition = currentTimeRounded;
                _mediator.Send(new GameEndConditionChangedMessage(
                    functionStartTime,
                    _settings.Objective.EndConditionCount,
                    currentTimeRounded,
                    _settings.Objective.GameEndCondition.ToString()));
            }

            if (!(functionStartTime > _settings.Objective.EndConditionCount)) return;
            EndGame(_settings.Objective.EndConditionCount);
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            var endCondition = _settings.Objective.GameEndCondition;
            var endConditionCount = _settings.Objective.EndConditionCount;
            switch (endCondition)
            {
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
                case GameEndCondition.Time:
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