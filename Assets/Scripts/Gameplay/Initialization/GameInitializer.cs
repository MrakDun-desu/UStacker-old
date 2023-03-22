using System;
using UnityEngine;
using UStacker.Gameplay.GameStateManagement;
using UStacker.Gameplay.InputProcessing;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.Music;

namespace UStacker.Gameplay.Initialization
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private MusicPlayerFinder _playerFinder;
        [SerializeField] private GameObject[] _gameSettingsDependencies = Array.Empty<GameObject>();
        [SerializeField] private RotationSystemSO _srsRotationSystemSo;
        [SerializeField] private RotationSystemSO _srsPlusRotationSystemSo;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private GameStateManager _stateManager;

        private static GameSettingsSO.SettingsContainer _gameSettings;
        private static GameReplay _replay;

        public static GameSettingsSO.SettingsContainer GameSettings
        {
            get => _gameSettings;
            set
            {
                _gameSettings = value;
                if (!AppSettings.GameOverrides.TryGetValue(GameType, out var overrides)) return;
                if (overrides.CountdownCount is { } count)
                    GameSettings.Presentation.CountdownCount = count;
                if (overrides.CountdownInterval is { } interval)
                    GameSettings.Presentation.CountdownInterval = interval;
                if (overrides.StartingLevel is { } startingLevel)
                    GameSettings.Objective.StartingLevel = startingLevel;
                
                GameSettings.Presentation.StatCounterGroupOverrideId = overrides.StatCounterGroupId;

                if (!GameSettings.Controls.OverrideHandling && _replay is null)
                    GameSettings.Controls.Handling.Override(AppSettings.Handling);
            }
        }

        public static GameReplay Replay
        {
            get => _replay;
            set
            {
                _replay = value;
                if (_replay is not null)
                    GameSettings = _replay.GameSettings;
            }
        }

        public static string GameType { get; set; }

        private void Start()
        {
            if (Replay is not null)
            {
                _inputProcessor.PlacementsList = Replay.PiecePlacementList;
                _inputProcessor.ActionList = Replay.ActionList;
            }
            GameSettings.Controls.ActiveRotationSystem =
            GameSettings.Controls.RotationSystemType switch
            {
                RotationSystemType.SRS => _srsRotationSystemSo.RotationSystem,
                RotationSystemType.SRSPlus => _srsPlusRotationSystemSo.RotationSystem,
                RotationSystemType.None => new RotationSystem(),
                RotationSystemType.Custom => GameSettings.Controls.ActiveRotationSystem,
                _ => new RotationSystem()
            };

            _playerFinder.GameType = GameType;
            _stateManager.GameType = GameType;
            foreach (var dependantObject in _gameSettingsDependencies)
            {
                var dependencies = dependantObject.GetComponents<IGameSettingsDependency>();
                foreach (var dependency in dependencies)
                    dependency.GameSettings = GameSettings;
            }
            
            GameStateChangeEventReceiver.Activate();
        }

        private void OnDestroy()
        {
            GameStateChangeEventReceiver.Deactivate();
        }
    }
}