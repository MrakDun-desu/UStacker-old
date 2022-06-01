using System.IO;
using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesLevellingInitializer : InitializerBase
    {
        public RulesLevellingInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings
        ) : base(problemBuilder, gameSettings)
        {
        }

        private static string LevellingSystemsPath
            => Path.Combine(Application.persistentDataPath, "levellingSystems");

        public override void Execute()
        {
            // TODO add levelling systems
            if (_gameSettings.Rules.Levelling.LevellingSystem ==
                LevellingSystem.Custom)
            {
                var levellingSystemPath = Path.Combine(
                    LevellingSystemsPath,
                    _gameSettings.Rules.Levelling.CustomLevellingScriptName
                );

                if (!File.Exists(levellingSystemPath))
                {
                    _errorBuilder.AppendLine("Custom levelling system not found.");
                    return;
                }

                _gameSettings.Rules.Levelling.CustomLevellingScript =
                    File.ReadAllText(levellingSystemPath);
            }
        }
    }
}