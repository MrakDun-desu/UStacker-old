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
        [SerializeField] private Piece[] _availablePieces = new Piece[0];
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private KickSystemSO _srsKickSystemSO;
        [SerializeField] private KickSystemSO _srsPlusKickSystemSO;
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

        public bool TryInitialize(StringBuilder errorBuilder)
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
                    _srsKickSystemSO.KickSystem,
                    _srsPlusKickSystemSO.KickSystem),
                new PresentationInitializer(
                    errorBuilder, _gameSettingsAsset,
                    _gameTitle,
                    _countdown
                )
            };

            for (var i = 0; i < initializers.Count; i++) {
                initializers[i].Execute();
            }

            if (errorBuilder.Length > 0) return false;
            return true;
        }
    }
}