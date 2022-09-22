using System;
using System.Collections.Generic;
using System.Text;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Presentation;
using Blockstacker.GameSettings;
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

        [Header("Events")] public UnityEvent GameInitialized;
        public UnityEvent GameReinitialized;
        public UnityEvent<string> GameFailedToInitialize;

        private void Start()
        {
            _loadingOverlay.SetActive(true);
            StringBuilder errorBuilder = new();
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
                GameReinitialized.Invoke();
                _loadingOverlay.SetActive(false);
                return;
            }

            GameFailedToInitialize.Invoke("Game failed to initialize: \n" + errorBuilder);
        }

        private bool TryInitialize(StringBuilder errorBuilder)
        {
            List<InitializerBase> initializers = new()
            {
                new RulesBoardDimensionsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _board,
                    _boardBackground,
                    _gridBlock,
                    _gridPrefab,
                    _statsCanvasTransform,
                    Camera.main
                ),
                new RulesGeneralInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _availablePieces,
                    _pieceSpawner,
                    _board,
                    _pieceContainerPrefab,
                    _inputProcessor),
                new RulesHandlingInitializer(errorBuilder, _gameSettingsAsset),
                new RulesControlsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _srsRotationSystemSo.RotationSystem,
                    _srsPlusRotationSystemSo.RotationSystem,
                    _inputProcessor),
                new GarbageGenerationInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _board),
                new PresentationInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _gameTitle,
                    _countdown
                )
            };

            foreach (var initializer in initializers) initializer.Execute();

            return errorBuilder.Length <= 0;
        }

        private bool TryReinitialize(StringBuilder errorBuilder)
        {
            List<InitializerBase> initializers = new()
            {
                new RulesGeneralInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _availablePieces,
                    _pieceSpawner,
                    _board,
                    _pieceContainerPrefab,
                    _inputProcessor,
                    true),
                new RulesHandlingInitializer(errorBuilder, _gameSettingsAsset),
                new RulesControlsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _srsRotationSystemSo.RotationSystem,
                    _srsPlusRotationSystemSo.RotationSystem,
                    _inputProcessor),
                new PresentationInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _gameTitle,
                    _countdown
                )
            };

            foreach (var initializer in initializers) initializer.Execute();

            return errorBuilder.Length <= 0;
        }
    }
}