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

        public ObjectiveInitializer(StringBuilder errorBuilder, GameSettingsSO gameSettings, MediatorSO mediator) :
            base(errorBuilder, gameSettings)
        {
            _mediator = mediator;
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

            var managerObject = new GameObject();
            IGameManager manager = _gameSettings.Objective.GameManagerType switch
            {
                GameManagerType.None => null,
                GameManagerType.Modern => managerObject.AddComponent<ModernGameManager>(),
                GameManagerType.Classic => managerObject.AddComponent<ClassicGameManager>(),
                GameManagerType.Custom => managerObject.AddComponent<CustomGameManager>(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (manager is null)
            {
                Object.Destroy(managerObject.gameObject);
                return;
            }

            manager.Initialize(_gameSettings.Objective.StartingLevel, _mediator);
        }
    }
}