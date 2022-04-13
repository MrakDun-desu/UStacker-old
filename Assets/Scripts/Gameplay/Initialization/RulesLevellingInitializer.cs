using System.IO;
using System.Text;
using Blockstacker.Gameplay.Levelling;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesLevellingInitializer : InitializerBase
    {
        private static string LevellingSystemsPath
            => Path.Combine(Application.persistentDataPath, "levellingSystems");
        private GameManager _manager;

        public RulesLevellingInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings,
            GameManager manager) : base(problemBuilder, gameSettings)
        {
            _manager = manager;
        }

        public override void Execute()
        {
            if (_gameSettings.Rules.Levelling.LevellingSystem ==
                LevellingSystem.Custom) {
                var levellingSystemPath = Path.Combine(
                    LevellingSystemsPath,
                    _gameSettings.Rules.Levelling.CustomLevellingScriptName
                );

                if (!File.Exists(levellingSystemPath)) {
                    _errorBuilder.AppendLine("Custom levelling system not found.");
                    return;
                }

                _gameSettings.Rules.Levelling.CustomLevellingScript =
                    File.ReadAllText(levellingSystemPath);
            }

            bool isValid = true;
            // TODO
            // _manager.levellingSystem = _gameSettings.Rules.Levelling.LevellingSystem switch
            // {
            // };

            if (!isValid) {
                _errorBuilder.AppendLine("Custom levelling system is not valid.");
            }
        }
    }
}