using System;
using System.Collections.Generic;
using System.Text;
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
        [SerializeField] private Piece[] _availablePieces = Array.Empty<Piece>();
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private PieceSpawner _pieceSpawner;
        [SerializeField] private Board _board;
        [SerializeField] private GameObject _boardBackground;
        [SerializeField] private RotationSystemSO _srsRotationSystemSo;
        [SerializeField] private RotationSystemSO _srsPlusRotationSystemSo;
        [SerializeField] private GameCountdown _countdown;
        [SerializeField] private TMP_Text _gameTitle;
        [SerializeField] private GameObject _gridPiece;

        public UnityEvent GameInitialized;
        public UnityEvent<string> GameFailedToInitialize;

        private void Start()
        {
            StringBuilder errorBuilder = new();
            if (TryInitialize(errorBuilder)) {
                GameInitialized.Invoke();
                return;
            }

            GameFailedToInitialize.Invoke("Game failed to initialize:\n" + errorBuilder);
        }

        private bool TryInitialize(StringBuilder errorBuilder)
        {
            List<InitializerBase> initializers = new()
            {
                new RulesGeneralInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _availablePieces.Length,
                    _pieceSpawner,
                    _availablePieces),
                new RulesHandlingInitializer(errorBuilder, _gameSettingsAsset),
                new RulesControlsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _srsRotationSystemSo._rotationSystem,
                    _srsPlusRotationSystemSo._rotationSystem),
                new PresentationInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _gameTitle,
                    _countdown
                ),
                new RulesBoardDimensionsInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _board,
                    _boardBackground,
                    _gridPiece,
                    Camera.main 
                    )
            };

            foreach (var initializer in initializers)
            {
                initializer.Execute();
            }
            
            Mediator.Clear();

            return errorBuilder.Length <= 0;
        }
    }
}