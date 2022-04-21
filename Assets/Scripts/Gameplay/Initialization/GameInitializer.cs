using System;
using System.Collections.Generic;
using System.Text;
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
        [SerializeField] private Piece[] _availablePieces = Array.Empty<Piece>();
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private RotationSystemSO _srsRotationSystemSo;
        [SerializeField] private RotationSystemSO _srsPlusRotationSystemSo;
        [SerializeField] private GameCountdown _countdown;
        [SerializeField] private TMP_Text _gameTitle;


        public static event Action GameInitialized;
        public UnityEvent<string> GameFailedToInitialize;

        private void Start()
        {
            StringBuilder errorBuilder = new();
            if (TryInitialize(errorBuilder)) {
                GameInitialized?.Invoke();
                return;
            }

            GameFailedToInitialize.Invoke(errorBuilder.ToString());
        }

        private bool TryInitialize(StringBuilder errorBuilder)
        {
            List<InitializerBase> initializers = new()
            {
                new RulesGeneralInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _availablePieces.Length,
                    _gameManager),
                new RulesHandlingInitializer(errorBuilder, _gameSettingsAsset),
                new RulesControlsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _srsRotationSystemSo._rotationSystem,
                    _srsPlusRotationSystemSo._rotationSystem),
                new PresentationInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _gameTitle,
                    _countdown
                )
            };

            foreach (var initializer in initializers)
            {
                initializer.Execute();
            }

            return errorBuilder.Length <= 0;
        }
    }
}