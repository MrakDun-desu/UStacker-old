using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.Blocks;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.InputProcessing;
using UStacker.Gameplay.Pieces;
using UStacker.Gameplay.Presentation;
using UStacker.Gameplay.Stats;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GlobalSettings.Music;

namespace UStacker.Gameplay.Initialization
{
    public class GameInitializer : MonoBehaviour
    {
        private static GameReplay _replay;
        [Space][SerializeField] private PieceDictionary _availablePieces = new();

        [Header("Board")][SerializeField] private PieceSpawner _pieceSpawner;
        [SerializeField] private Board _board;
        [SerializeField] private RectTransform _statsCanvasTransform;
        [SerializeField] private GameObject _boardBackground;
        [SerializeField] private BlockBase _gridBlock;
        [SerializeField] private BoardGrid _gridPrefab;
        [SerializeField] private PieceContainer _pieceContainerPrefab;
        [SerializeField] private InputProcessor _inputProcessor;

        [Header("Rotation systems")]
        [SerializeField]
        private RotationSystemSO _srsRotationSystemSo;
        [SerializeField] private RotationSystemSO _srsPlusRotationSystemSo;

        [Header("Others")][SerializeField] private GameCountdown _countdown;
        [SerializeField] private TMP_Text _gameTitle;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private GameStateManager _stateManager;
        [SerializeField] private MusicPlayerFinder _playerFinder;
        [SerializeField] private StatCounterManager _statCounterManager;
        [SerializeField] private ReplayController _replayController;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private GameObject[] _gameSettingsDependencies = Array.Empty<GameObject>();

        [Header("Events")] public UnityEvent GameInitialized;
        public UnityEvent ReplayStarted;
        public UnityEvent ReplayInitialized;
        public UnityEvent<string> GameFailedToInitialize;
        public static GameSettingsSO.SettingsContainer GameSettings { get; set; }

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
        public static bool InitAsReplay { get; set; }

        private void Awake()
        {
            foreach (var dependantObject in _gameSettingsDependencies)
            {
                var dependencies = dependantObject.GetComponents<IGameSettingsDependency>();
                foreach (var dependency in dependencies)
                    dependency.GameSettings = GameSettings;
            }
        }

        private void Start()
        {
            _loadingOverlay.SetActive(true);
            StringBuilder errorBuilder = new();
            
            if (TryInitialize(errorBuilder))
            {
                if (InitAsReplay)
                    ReplayInitialized.Invoke();
                else
                    GameInitialized.Invoke();
                _loadingOverlay.SetActive(false);
                return;
            }

            GameFailedToInitialize.Invoke("Game failed to initialize:\n" + errorBuilder);
        }

        public void Restart()
        {
            _loadingOverlay.SetActive(true);

            StringBuilder errorBuilder = new();
            if (TryReinitialize(errorBuilder))
            {
                if (InitAsReplay)
                {
                    ReplayStarted.Invoke();
                    _replayController.SetReplay(Replay);
                }
                else
                    GameInitialized.Invoke();
                _loadingOverlay.SetActive(false);
                return;
            }

            GameFailedToInitialize.Invoke("Game failed to initialize: \n" + errorBuilder);
        }

        private bool TryInitialize(StringBuilder errorBuilder)
        {
            _playerFinder.GameType = GameType;
            _statCounterManager.GameType = GameType;
            _stateManager.GameType = GameType;
            List<InitializerBase> initializers = new()
            {
                new OverridesInitializer(errorBuilder, GameSettings, GameType, InitAsReplay),
                new BoardDimensionsInitializer(
                    errorBuilder, GameSettings,
                    _board,
                    _boardBackground,
                    _gridBlock,
                    _gridPrefab,
                    _statsCanvasTransform,
                    Camera.main
                ),
                new GeneralInitializer(
                    errorBuilder, GameSettings,
                    _availablePieces,
                    _pieceSpawner,
                    _board,
                    _pieceContainerPrefab,
                    _inputProcessor,
                    _stateManager,
                    Replay,
                    false),
                new ControlsInitializer(
                    errorBuilder, GameSettings,
                    _srsRotationSystemSo.RotationSystem,
                    _srsPlusRotationSystemSo.RotationSystem,
                    _inputProcessor,
                    InitAsReplay),
                new PresentationInitializer(
                    errorBuilder, GameSettings,
                    _gameTitle,
                    _countdown
                ),
                new ObjectiveInitializer(
                    errorBuilder, GameSettings,
                    _mediator,
                    _stateManager,
                    _board,
                    _timer)
            };

            foreach (var initializer in initializers) initializer.Execute();

            return errorBuilder.Length <= 0;
        }

        private bool TryReinitialize(StringBuilder errorBuilder)
        {
            _playerFinder.GameType = GameType;

            List<InitializerBase> initializers = new()
            {
                new GeneralInitializer(
                    errorBuilder, GameSettings,
                    _availablePieces,
                    _pieceSpawner,
                    _board,
                    _pieceContainerPrefab,
                    _inputProcessor,
                    _stateManager,
                    Replay,
                    true)
            };

            foreach (var initializer in initializers) initializer.Execute();

            return errorBuilder.Length <= 0;
        }
    }
}