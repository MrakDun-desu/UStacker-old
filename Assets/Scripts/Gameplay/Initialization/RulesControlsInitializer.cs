using System.IO;
using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesControlsInitializer : InitializerBase
    {
        private readonly RotationSystem _srsRotationSystem;
        private readonly RotationSystem _srsPlusRotationSystem;
        private readonly InputProcessor _inputProcessor;

        private static string KickSystemsPath => Path.Combine(
            Application.persistentDataPath, "ruleCustomization/rotationSystems"
        );

        public RulesControlsInitializer(
            StringBuilder errorBuilder,
            GameSettingsSO gameSettings,
            RotationSystem srsRotationSystem,
            RotationSystem srsPlusRotationSystem,
            InputProcessor inputProcessor)
            : base(errorBuilder, gameSettings)
        {
            _srsRotationSystem = srsRotationSystem;
            _srsPlusRotationSystem = srsPlusRotationSystem;
            _inputProcessor = inputProcessor;
        }

        public override void Execute()
        {
            RotationSystem customSystem = new();
            if (_gameSettings.Rules.Controls.RotationSystem ==
                RotationSystemType.Custom) {

                var kickTablePath = Path.Combine(KickSystemsPath,
                 _gameSettings.Rules.General.CustomRandomizerName);
                if (!File.Exists(kickTablePath)) {
                    _errorBuilder.AppendLine("Custom rotation system not found.");
                    return;
                }
                JsonUtility.FromJsonOverwrite(File.ReadAllText(kickTablePath), customSystem);
            }

            _gameSettings.Rules.Controls.ActiveRotationSystem =
                _gameSettings.Rules.Controls.RotationSystem switch
                {
                    RotationSystemType.SRS => _srsRotationSystem,
                    RotationSystemType.SRSPlus => _srsPlusRotationSystem,
                    RotationSystemType.None => new RotationSystem(),
                    RotationSystemType.Custom => customSystem,
                    _ => new RotationSystem()
                };

            if (!_gameSettings.Rules.Controls.AllowHold)
                _inputProcessor.DeactivateHold();

            _inputProcessor.KickHandler = new KickHandler(_gameSettings.Rules.Controls.ActiveRotationSystem);

        }
    }
}
