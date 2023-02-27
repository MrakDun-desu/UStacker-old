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

        public GameSettingsSO.SettingsContainer GameSettings { private get; set; }

        private void Awake()
        {
            IGameManager manager = GameSettings.Objective.GameManagerType switch
            {
                GameManagerType.None => null,
                GameManagerType.ModernWithLevelling => gameObject.AddComponent<ModernGameManagerWithLevelling>(),
                GameManagerType.ModernWithoutLevelling =>
                    gameObject.AddComponent<ModernGameManagerWithoutLevelling>(),
                GameManagerType.Classic => gameObject.AddComponent<ClassicGameManager>(),
                GameManagerType.Custom => gameObject.AddComponent<CustomGameManager>(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (manager is null)
            {
                Destroy(gameObject);
                return;
            }

            manager.Initialize(GameSettings.Objective.StartingLevel, _mediator);

            if (manager is CustomGameManager custom)
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