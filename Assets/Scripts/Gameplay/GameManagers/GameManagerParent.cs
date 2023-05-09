
/************************************
GameManagerParent.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;

namespace UStacker.Gameplay.GameManagers
{
    public class GameManagerParent : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private UnityEvent<double> OnGameEnded;
        [SerializeField] private UnityEvent<double> OnGameLost;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private Board _board;
        [SerializeField] private GameTimer _timer;

        private IGameManager _currentManager;
        private GameSettingsSO.SettingsContainer _gameSettings;

        public GameSettingsSO.SettingsContainer GameSettings
        {
            private get => _gameSettings;
            set
            {
                _gameSettings = value;
                Initialize();
            }
        }

        private void Initialize()
        {
            _currentManager?.Delete();

            _currentManager = GameSettings.Objective.GameManagerType switch
            {
                GameManagerType.None => null,
                GameManagerType.ModernWithLevelling => gameObject.AddComponent<ModernGameManagerWithLevelling>(),
                GameManagerType.ModernWithoutLevelling =>
                    gameObject.AddComponent<ModernGameManagerWithoutLevelling>(),
                GameManagerType.Classic => gameObject.AddComponent<ClassicGameManager>(),
                GameManagerType.Custom => gameObject.AddComponent<CustomGameManager>(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_currentManager is null)
                return;

            _currentManager.Initialize(GameSettings.Objective.StartingLevel, _mediator);

            if (_currentManager is CustomGameManager custom)
                custom.CustomInitialize(
                    _board,
                    _timer,
                    GameSettings.Objective.CustomGameManagerScript,
                    OnGameEnded,
                    OnGameLost,
                    GameSettings.General.ActiveSeed);
        }
    }
}
/************************************
end GameManagerParent.cs
*************************************/
