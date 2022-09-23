using System.IO;
using System.Text;
using Blockstacker.Common;
using Blockstacker.Gameplay.GarbageGeneration;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.Gameplay.Initialization
{
    public class GarbageGenerationInitializer : InitializerBase
    {
        private readonly Board _board;
        
        public GarbageGenerationInitializer(
            StringBuilder errorBuilder, 
            GameSettingsSO gameSettings,
            Board board) 
            : base(errorBuilder, gameSettings)
        {
            _board = board;
        }

        public override void Execute()
        {
            if (!_gameSettings.Objective.UseCustomGarbageScript &&
                _gameSettings.Objective.GarbageGeneration == GameSettings.Enums.GarbageGeneration.None)
                return;
            
            _board.InitializeGarbagePools();
            
            var readonlyBoard = new GarbageBoardInterface(_board);
            if (_gameSettings.Objective.UseCustomGarbageScript)
            {
                var cheeseScriptPath = Path.Combine(
                    CustomizationPaths.CheeseGenerators,
                    _gameSettings.Objective.CustomGarbageScriptName
                );
                if (!File.Exists(cheeseScriptPath))
                {
                    _errorBuilder.AppendLine("Custom cheese generator script not found");
                    return;
                }

                _gameSettings.Objective.CustomGarbageScript = File.ReadAllText(cheeseScriptPath);

            }

            string validationErrors = null;

            IGarbageGenerator garbageGenerator = _gameSettings.Objective.UseCustomGarbageScript switch
            {
                false => new DefaultGarbageGenerator(
                    readonlyBoard,
                    _gameSettings.Objective.GarbageGeneration
                    ),
                true => new CustomGarbageGenerator(
                    readonlyBoard,
                    _gameSettings.Objective.CustomGarbageScript,
                    out validationErrors)
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