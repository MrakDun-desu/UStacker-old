using System;
using System.Text;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.GameManagers;
using Blockstacker.Gameplay.GarbageGeneration;
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
            InitializeGameManager();
            InitializeGarbageGenerator();
        }
        
        private void InitializeGameManager()
        {
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

        private void InitializeGarbageGenerator()
        {
            if (_gameSettings.Objective.GarbageGeneration == GameSettings.Enums.GarbageGeneration.None)
                return;
            
            _board.InitializeGarbagePools();
            
            var readonlyBoard = new GarbageBoardInterface(_board);
            string validationErrors = null;

            IGarbageGenerator garbageGenerator = _gameSettings.Objective.GarbageGeneration switch
            {
                GameSettings.Enums.GarbageGeneration.Custom => new CustomGarbageGenerator(
                    readonlyBoard,
                    _gameSettings.Objective.CustomGarbageScript,
                    out validationErrors),
                _ => new DefaultGarbageGenerator(
                    readonlyBoard,
                    _gameSettings.Objective.GarbageGeneration
                    )
            };

            if (validationErrors is not null)
            {
                _errorBuilder.AppendLine(validationErrors);
                return;
            }
            _board.GarbageGenerator = garbageGenerator;
        }
    }
}