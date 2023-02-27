using System;
using UnityEngine;
using UStacker.GameSettings;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.Music;

namespace UStacker.Gameplay.Initialization
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private MusicPlayerFinder _playerFinder;
        [SerializeField] private GameObject[] _gameSettingsDependencies = Array.Empty<GameObject>();

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
            }
        }

        public static GameReplay Replay
        {
            get => _replay;
            set
            {
                _replay = value;
                if (_replay is not null)
                    GameSettings = _replay?.GameSettings;
            }
        }

        public static string GameType { get; set; }

        private void Awake()
        {
            _playerFinder.GameType = GameType;
            foreach (var dependantObject in _gameSettingsDependencies)
            {
                var dependencies = dependantObject.GetComponents<IGameSettingsDependency>();
                foreach (var dependency in dependencies)
                    dependency.GameSettings = GameSettings;
            }
        }
    }
}