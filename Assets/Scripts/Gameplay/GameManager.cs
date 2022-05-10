using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Blockstacker.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _settings;
        
        [SerializeField] private UnityEvent GameStarted;
        [SerializeField] private UnityEvent GamePaused;
        [SerializeField] private UnityEvent GameResumed;
        [SerializeField] private UnityEvent GameRestarted;
        [SerializeField] private UnityEvent GameLost;
        [SerializeField] private UnityEvent GameEnded;

        private bool _gameRunning;

        public void StartGame()
        {
            _gameRunning = true;
            GameStarted.Invoke();
        }

        public void TogglePause()
        {
            if (_gameRunning)
            {
                Debug.Log("Paused");
                GamePaused.Invoke();
            }
            else
            {
                Debug.Log("Resumed");
                GameResumed.Invoke();
            }

            _gameRunning = !_gameRunning;
        }
        
        public void Restart() => GameRestarted.Invoke();

        public void EndGame()
        {
            if (_settings.Objective.ToppingOutIsOkay)
                GameEnded.Invoke();
            else
                GameLost.Invoke();
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
    }
}