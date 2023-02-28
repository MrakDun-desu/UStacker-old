using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Stats;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GlobalSettings;
using Logger = UStacker.Common.Logger;

namespace UStacker.Gameplay.GameStateManagement
{
    public class GameStateManager : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private GameTimer _timer;
        [SerializeField] private StatCounterManager _statCounterManager;
        [SerializeField] private GameRecorder _gameRecorder;
        [SerializeField] private GameResultDisplayer _resultDisplayer;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private GameObject _crashCanvas;
        [SerializeField] private TMP_Text _crashMessage;
        [SerializeField] private TMP_Text _crashCountdown;
        [SerializeField] private Button _exitButton;

        private readonly GameReplay Replay = new();
        private GameSettingsSO.SettingsContainer _replaySettings;
        private GameSettingsSO.SettingsContainer _settings;

        private GameState _currentState = GameState.Unset;
        private GameState _previousState;

        private GameState CurrentState
        {
            get => _currentState;
            set
            {
                _previousState = _currentState;
                _currentState = value;
            }
        }

        public static bool IsReplay { get; set; }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            private get => _settings;
            set
            {
                _settings = value;
                _replaySettings = value with { };
            }
        }

        public void InitializeGame()
        {
            _mediator.Send(new GameStateChangedMessage(
                _currentState,
                GameState.Initializing,
                0, IsReplay));
            _mediator.Send(new GameStateChangedMessage(
                GameState.Initializing,
                GameState.StartCountdown,
                0, IsReplay));
        }

        public void StartGame()
        {
            if (CurrentState is
                GameState.StartCountdown or
                GameState.ResumeCountdown)
            {
                _mediator.Send(new GameStateChangedMessage(
                    _currentState,
                    GameState.Running,
                    _timer.CurrentTime,
                    IsReplay));
            }
            else
                Debug.LogWarning("Trying to start game from invalid state " + CurrentState);
        }

        public void PauseGameIfRunning()
        {
            if (CurrentState is
                GameState.Initializing or
                GameState.Running or
                GameState.ResumeCountdown or
                GameState.StartCountdown)
                _mediator.Send(
                    new GameStateChangedMessage(CurrentState, GameState.Paused, _timer.CurrentTime, IsReplay));
        }

        public void TogglePause()
        {
            GameState newState;
            switch (CurrentState)
            {
                case GameState.Unset or
                    GameState.Any or
                    GameState.Lost:
                    Debug.LogWarning("Trying to toggle pause in invalid state " + CurrentState);
                    return;
                case GameState.Ended:
                    // in ended we can't pause, but we don't want do throw an error because we might try
                    return;
                case GameState.Paused:
                    switch (_previousState)
                    {
                        case GameState.Initializing or GameState.StartCountdown:
                            newState = GameState.StartCountdown;
                            break;
                        case GameState.Running or GameState.ResumeCountdown:
                            newState = GameState.ResumeCountdown;
                            break;
                        default:
                            Debug.LogWarning("Trying to unpause game with invalid previous state " + _previousState);
                            return;
                    }

                    break;
                case GameState.Initializing or
                    GameState.Running or
                    GameState.ResumeCountdown or
                    GameState.StartCountdown:
                    newState = GameState.Paused;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _mediator.Send(new GameStateChangedMessage(
                _currentState,
                newState,
                _timer.CurrentTime,
                IsReplay));
        }

        public void LoseGame(double loseTime)
        {
            if (GameSettings.Objective.ToppingOutIsOkay)
            {
                EndGame(loseTime);
                return;
            }

            if (CurrentState == GameState.Running)
            {
                _mediator.Send(new GameStateChangedMessage(
                    _currentState,
                    GameState.Lost,
                    loseTime,
                    IsReplay));
            }
            else
                Debug.LogWarning("Trying to lose game from invalid state " + CurrentState);
        }

        public void EndGame(double endTime)
        {
            switch (CurrentState)
            {
                case GameState.Unset or
                    GameState.Any or
                    GameState.Initializing or
                    GameState.StartCountdown or
                    GameState.ResumeCountdown or
                    GameState.Ended or
                    GameState.Lost:
                    Debug.LogWarning("Trying to end game from invalid state " + CurrentState);
                    return;
                case GameState.Paused:
                    if (!IsReplay)
                    {
                        Debug.LogWarning("Trying to end game from invalid state " + CurrentState);
                        return;
                    }

                    break;
                case GameState.Running:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!IsReplay)
            {
                _gameRecorder.ActionList.Sort((a, b) => a.Time.CompareTo(b.Time));
                _gameRecorder.PiecePlacementList.Sort((a, b) => a.PlacementTime.CompareTo(b.PlacementTime));
                Replay.GameType = GameInitializer.GameType;
                Replay.GameSettings = _replaySettings;
                Replay.ActionList.Clear();
                Replay.PiecePlacementList.Clear();
                Replay.ActionList.AddRange(_gameRecorder.ActionList);
                Replay.PiecePlacementList.AddRange(_gameRecorder.PiecePlacementList);
                Replay.Stats = _statCounterManager.Stats;
                Replay.GameLength = endTime;
                Replay.TimeStamp = DateTime.UtcNow;

                _resultDisplayer.DisplayedReplay = Replay;

                if (AppSettings.Gameplay.AutosaveReplaysOnDisk)
                    Replay.Save();
            }

            _mediator.Send(new GameStateChangedMessage(
                _currentState, GameState.Ended, endTime, IsReplay));
        }

        public void EndReplay()
        {
            if (!IsReplay)
                return;

            if (CurrentState is GameState.Running or GameState.Paused)
                _mediator.Send(new GameStateChangedMessage(
                    _currentState, GameState.Ended, Replay.GameLength, IsReplay));
            else
                Debug.LogWarning("Trying to end game from");
        }

        private void CrashGame(GameCrashedMessage message)
        {
            Logger.Log("CRASH: " + message.CrashMessage);
            StartCoroutine(CrashGameCor(message.CrashMessage));
        }

        private IEnumerator CrashGameCor(string crashMessage)
        {
            _crashCanvas.SetActive(true);
            _exitButton.Select();

            for (var countdown = 20; countdown > 0; countdown--)
            {
                _crashMessage.text = crashMessage;
                _crashCountdown.text = $"Exiting in {countdown}...";
                yield return new WaitForSeconds(1);
            }

            SceneManager.LoadScene("Scene_Menu_Main");
        }

        public void TogglePause(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                TogglePause();
        }

        public void Restart(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                InitializeGame();
        }

        private void Start()
        {
            InitializeGame();
        }

        private void OnSeedSet(SeedSetMessage message)
        {
            _replaySettings.General.ActiveSeed = message.Seed;
        }

        private void Awake()
        {
            _exitButton.onClick.AddListener(() => SceneManager.LoadScene("Scene_Menu_Main"));
            _mediator.Register<GameCrashedMessage>(CrashGame);
            _mediator.Register<SeedSetMessage>(OnSeedSet);
            _mediator.Register<GameStateChangedMessage>(message => CurrentState = message.NewState, 10);
            AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocusChanged += OnPauseSettingChange;
            OnPauseSettingChange(AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocus);
        }

        private void OnDestroy()
        {
            AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocusChanged -= OnPauseSettingChange;
            Application.focusChanged -= OnFocusChanged;
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
            if (!hasFocus && CurrentState is GameState.Running or GameState.Initializing)
                TogglePause();
        }
    }
}