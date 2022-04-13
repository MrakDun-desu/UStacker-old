using System.IO;
using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesLevellingInitializer : InitializerBase
    {
        private static string LevellingSystemsPath
            => Path.Combine(Application.persistentDataPath, "levellingSystems");
        public RulesLevellingInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings
            ) : base(problemBuilder, gameSettings)
        {
        }

        public override void Execute()
        {
            // TODO
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

        }
    }
}