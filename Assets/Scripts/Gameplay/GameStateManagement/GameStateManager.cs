
/************************************
GameStateManager.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;

namespace UStacker.Gameplay.GameStateManagement
{
    public class GameStateManager : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private GameTimer _timer;
        [SerializeField] private Mediator _mediator;

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

        public bool IsReplay { get; set; }

        private void OnEnable()
        {
            _mediator.Register<GameStateChangedMessage>(UpdateGameState, 10);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GameStateChangedMessage>(UpdateGameState);
        }

        public GameSettingsSO.SettingsContainer GameSettings { get; set; }

        // this method is split into 2 because of UnityEvents not working correctly with
        // methods that have default params
        public void InitializeGame()
        {
            _mediator.Send(new GameStateChangedMessage(
                _currentState,
                GameState.Initializing,
                0, IsReplay));

            StartCountdown();
        }

        public void InitializeGameWithoutCountdown()
        {
            _mediator.Send(new GameStateChangedMessage(
                _currentState,
                GameState.Initializing,
                0, IsReplay));
        }

        public void StartCountdown()
        {
            if (CurrentState != GameState.Initializing)
            {
                Debug.LogWarning("Trying to start countdown from invalid state " + CurrentState);
                return;
            }

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
                _mediator.Send(new GameStateChangedMessage(
                    _currentState,
                    GameState.Running,
                    _timer.CurrentTime,
                    IsReplay));
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
                    GameState.Lost or
                    GameState.Ended:
                    Debug.LogWarning("Trying to toggle pause in invalid state " + CurrentState);
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
                _mediator.Send(new GameStateChangedMessage(
                    _currentState,
                    GameState.Lost,
                    loseTime,
                    IsReplay));
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

            _mediator.Send(new GameStateChangedMessage(
                _currentState, GameState.Ended, endTime, IsReplay));
        }

        public void EndReplay(double endTime)
        {
            if (!IsReplay)
                return;

            if (CurrentState is GameState.Running or GameState.Paused)
                _mediator.Send(new GameStateChangedMessage(
                    _currentState, GameState.Ended, endTime, IsReplay));
            else
                Debug.LogWarning("Trying to end replay from invalid state " + CurrentState);
        }

        public void TogglePause(InputAction.CallbackContext ctx)
        {
            if (ctx.performed &&
                CurrentState is GameState.Paused
                    or GameState.Running
                    or GameState.StartCountdown
                    or GameState.ResumeCountdown
                    or GameState.Initializing)
                TogglePause();
        }

        public void Restart(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                InitializeGame();
        }

        private void UpdateGameState(GameStateChangedMessage message)
        {
            CurrentState = message.NewState;
        }
    }
}
/************************************
end GameStateManager.cs
*************************************/
