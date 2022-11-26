using System;
using System.Collections.Generic;
using System.Text;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Presentation;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings.Music;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Initialization
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _gameSettingsAsset;

        [Space] [SerializeField] private PieceDictionary _availablePieces = new();

        [Header("Board")] [SerializeField] private PieceSpawner _pieceSpawner;
        [SerializeField] private Board _board;
        [SerializeField] private RectTransform _statsCanvasTransform;
        [SerializeField] private GameObject _boardBackground;
        [SerializeField] private BlockBase _gridBlock;
        [SerializeField] private BoardGrid _gridPrefab;
        [SerializeField] private PieceContainer _pieceContainerPrefab;
        [SerializeField] private InputProcessor _inputProcessor;

        [Header("Rotation systems")] [SerializeField]
        private RotationSystemSO _srsRotationSystemSo;

        [SerializeField] private RotationSystemSO _srsPlusRotationSystemSo;

        [Header("Others")] [SerializeField] private GameCountdown _countdown;
        [SerializeField] private TMP_Text _gameTitle;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private GameStateManager _stateManager;
        [SerializeField] private MusicPlayerFinder _playerFinder;
        [SerializeField] private GameObject[] _gameSettingsDependencies = Array.Empty<GameObject>();

        [Header("Events")] public UnityEvent GameInitialized;
        public UnityEvent<string> GameFailedToInitialize;

        private void Start()
        {
            _loadingOverlay.SetActive(true);
            StringBuilder errorBuilder = new();
            foreach (var dependantObject in _gameSettingsDependencies)
            {
                var dependencies = dependantObject.GetComponents<IGameSettingsDependency>();
                foreach (var dependency in dependencies)
                    dependency.GameSettings = _gameSettingsAsset;
            }
            if (TryInitialize(errorBuilder))
            {
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
                GameInitialized.Invoke();
                _loadingOverlay.SetActive(false);
                return;
            }

            GameFailedToInitialize.Invoke("Game failed to initialize: \n" + errorBuilder);
        }

        private bool TryInitialize(StringBuilder errorBuilder)
        {
            _playerFinder.gameTypeStr = _gameSettingsAsset.GameType;
            List<InitializerBase> initializers = new()
            {
                new OverridesInitializer(errorBuilder, _gameSettingsAsset),
                new BoardDimensionsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _board,
                    _boardBackground,
                    _gridBlock,
                    _gridPrefab,
                    _statsCanvasTransform,
                    Camera.main
                ),
                new GeneralInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _availablePieces,
                    _pieceSpawner,
                    _board,
                    _pieceContainerPrefab,
                    _inputProcessor),
                new ControlsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _srsRotationSystemSo.RotationSystem,
                    _srsPlusRotationSystemSo.RotationSystem,
                    _inputProcessor),
                new PresentationInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _gameTitle,
                    _countdown
                ),
                new ObjectiveInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _mediator,
                    _stateManager,
                    _board)
            };

            foreach (var initializer in initializers) initializer.Execute();

            return errorBuilder.Length <= 0;
        }

        private bool TryReinitialize(StringBuilder errorBuilder)
        {
            List<InitializerBase> initializers = new()
            {
                new GeneralInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _availablePieces,
                    _pieceSpawner,
                    _board,
                    _pieceContainerPrefab,
                    _inputProcessor,
                    true),
            };

            foreach (var initializer in initializers) initializer.Execute();

            return errorBuilder.Length <= 0;
        }
    }
}