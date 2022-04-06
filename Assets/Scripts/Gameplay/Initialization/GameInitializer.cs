using System;
using System.Collections.Generic;
using System.Text;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GameSettings;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Initialization
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _gameSettingsAsset;
        [SerializeField] private Piece[] _availablePieces = new Piece[0];
        [SerializeField] private GameManager _gameManager;

        public static event Action GameInitialized;
        public UnityEvent<string> GameFailedToInitialize;

        private void Start()
        {
            StringBuilder errorBuilder = new();
            if (TryInitialize(errorBuilder)) {
                GameInitialized.Invoke();
                return;
            }

            GameFailedToInitialize.Invoke(errorBuilder.ToString());
        }

        public bool TryInitialize(StringBuilder errorBuilder)
        {
            List<InitializerBase> initializers = new();
            initializers.Add(new RulesGeneralInitializer(
                errorBuilder, _gameSettingsAsset,
                _availablePieces.Length,
                _gameManager));
            initializers.Add(new RulesHandlingInitializer(errorBuilder, _gameSettingsAsset));

            for (var i = 0; i < initializers.Count; i++) {
                initializers[i].Execute();
            }

            if (errorBuilder.Length > 0) return false;
            return true;
        }
    }
}