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
        
        [Space]
        [SerializeField] private UnityEvent GameStarted;
        [SerializeField] private UnityEvent GamePaused;
        [SerializeField] private UnityEvent GameResumed;
        [SerializeField] private UnityEvent GameRestarted;
        [SerializeField] private UnityEvent GameLost;
        [SerializeField] private UnityEvent GameEnded;

        public static event Action GameRestartedEvent;
        public static event Action GameEndedEvent;

        private bool _gameRunning;
        private bool _gameEnded;
        public GameReplay Replay;

        #region Game event management
        
        public void StartGame()
        {
            _gameRunning = true;
            Replay.GameSettings =
                JsonUtility.FromJson<GameSettingsSO.SettingsContainer>(JsonUtility.ToJson(_settings.Settings));
            GameStarted.Invoke();
        }

        public void TogglePause()
        {
            if (_gameEnded) return;
            
            if (_gameRunning)
            {
                GamePaused.Invoke();
            }
            else
            {
                GameResumed.Invoke();
            }

            _gameRunning = !_gameRunning;
        }

        public void Restart()
        {
            _gameEnded = false;
            GameRestarted.Invoke();
            GameRestartedEvent?.Invoke();
        }

        public void LoseGame()
        {
            _gameEnded = true;
            if (_settings.Objective.ToppingOutIsOkay)
                EndGame();
            else
                GameLost.Invoke();
        }

        public void EndGame()
        {
            _gameEnded = true;
            Replay.ActionList = new List<InputActionMessage>();
            Replay.ActionList.AddRange(_gameRecorder.ActionList);
            Replay.Stats = JsonUtility.FromJson<StatContainer>(JsonUtility.ToJson(_statCounter.Stats));
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
            if (_settings.Objective.GameEndCondition != GameEndCondition.Time) return;
            
            if (_timer.CurrentTime > _settings.Objective.EndConditionCount) EndGame();
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
                    if (_statCounter.LinesCleared >= _settings.Objective.EndConditionCount) 
                        EndGame();
                    break;
                case GameEndCondition.CheeseLinesCleared:
                    break;
                case GameEndCondition.PiecesPlaced:
                    if (_statCounter.PiecesPlaced >= _settings.Objective.EndConditionCount)
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