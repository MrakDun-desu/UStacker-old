using System;
using System.IO;
using System.Text;
using Blockstacker.Common;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.GameManagers;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Blockstacker.Gameplay.Initialization
{
    public class ObjectiveInitializer : InitializerBase
    {
        private readonly MediatorSO _mediator;
        private readonly GameStateManager _stateManager;
        private readonly Board _board;

        public ObjectiveInitializer(
            StringBuilder errorBuilder, GameSettingsSO gameSettings,
            MediatorSO mediator,
            GameStateManager stateManager,
            Board board) :
            base(errorBuilder, gameSettings)
        {
            _mediator = mediator;
            _stateManager = stateManager;
            _board = board;
        }

        public override void Execute()
        {
            if (_gameSettings.Objective.GameManagerType == GameManagerType.Custom)
            {
                var gameManagerPath = Path.Combine(CustomizationPaths.GameManagers,
                    _gameSettings.Objective.CustomGameManagerName);
                if (!File.Exists(gameManagerPath))
                    _errorBuilder.AppendLine("Custom game manager script not found!");
                _gameSettings.Objective.CustomGameManagerScript = File.ReadAllText(gameManagerPath);
            }

            var managerObject = new GameObject("GameManager");
            IGameManager manager = _gameSettings.Objective.GameManagerType switch
            {
                GameManagerType.None => null,
                GameManagerType.ModernWithLevelling => managerObject.AddComponent<ModernGameManagerWithLevelling>(),
                GameManagerType.ModernWithoutLevelling =>
                    managerObject.AddComponent<ModernGameManagerWithoutLevelling>(),
                GameManagerType.Classic => managerObject.AddComponent<ClassicGameManager>(),
                GameManagerType.Custom => managerObject.AddComponent<CustomGameManager>(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (manager == null)
            {
                Object.Destroy(managerObject.gameObject);
                return;
            }
            
            manager.Initialize(_gameSettings.Objective.StartingLevel, _mediator);
            
            if (manager is CustomGameManager custom)
                custom.CustomInitialize(_stateManager, _board, _gameSettings.Objective.CustomGameManagerScript);
        }
    }
}