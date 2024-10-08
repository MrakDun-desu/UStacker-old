
/************************************
GameInitializer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections;
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
        private static GameSettingsSO.SettingsContainer _gameSettings;
        private static GameReplay _replay;
        [SerializeField] private MusicPlayerFinder _playerFinder;
        [SerializeField] private RotationSystemSO _srsRotationSystemSo;
        [SerializeField] private RotationSystemSO _srsPlusRotationSystemSo;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private GameRecorder _recorder;
        [SerializeField] private GameStateManager _stateManager;

        private IGameSettingsDependency[] _gameSettingsDependencies;

        public static GameSettingsSO.SettingsContainer GameSettings
        {
            get => _gameSettings;
            set
            {
                _gameSettings = value;

                if (!_gameSettings.Controls.OverrideHandling && _replay is null)
                    _gameSettings.Controls.Handling.Override(AppSettings.Handling);

                if (!AppSettings.GameOverrides.TryGetValue(GameType, out var overrides)) return;
                if (overrides.CountdownCount is { } count)
                    _gameSettings.Presentation.CountdownCount = count;
                if (overrides.CountdownInterval is { } interval)
                    _gameSettings.Presentation.CountdownInterval = interval;
                if (overrides.StartingLevel is { } startingLevel)
                    _gameSettings.Objective.StartingLevel = startingLevel;

                _gameSettings.Presentation.StatCounterGroupOverrideId = overrides.StatCounterGroupId;
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

        private void Awake()
        {
            _gameSettingsDependencies = GetComponentsInChildren<IGameSettingsDependency>();
        }

        private void Start()
        {
            var isReplay = Replay is not null;
            _stateManager.IsReplay = isReplay;
            if (isReplay)
            {
                _inputProcessor.PlacementsList = Replay.PiecePlacementList;
                _inputProcessor.ActionList = Replay.ActionList;
                _recorder.Replay = Replay;
            }
            else
            {
                _recorder.GameType = GameType;
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

            foreach (var dependency in _gameSettingsDependencies)
                dependency.GameSettings = GameSettings;

            GameStateChangeEventReceiver.Activate();

            StartCoroutine(ScheduleGameStart());
        }

        private void OnDestroy()
        {
            GameStateChangeEventReceiver.Deactivate();
        }

        private IEnumerator ScheduleGameStart()
        {
            yield return new WaitForEndOfFrame();

            if (Replay is null)
                _stateManager.InitializeGame();
            else
                _stateManager.InitializeGameWithoutCountdown();
        }
    }
}
/************************************
end GameInitializer.cs
*************************************/
