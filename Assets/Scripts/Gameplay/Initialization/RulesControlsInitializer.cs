using System.IO;
using System.Text;
using Blockstacker.Common;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.Gameplay.Spins;
using Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesControlsInitializer : InitializerBase
    {
        private readonly InputProcessor _inputProcessor;
        private readonly RotationSystem _srsPlusRotationSystem;
        private readonly RotationSystem _srsRotationSystem;

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

        private static string KickSystemsPath => Path.Combine(
            Application.persistentDataPath, "ruleCustomization/rotationSystems"
        );

        public override void Execute()
        {
            RotationSystem customSystem = new();
            if (_gameSettings.Rules.Controls.RotationSystem ==
                RotationSystemType.Custom)
            {
                var kickTablePath = Path.Combine(KickSystemsPath,
                    _gameSettings.Rules.General.CustomRandomizerName);
                if (!File.Exists(kickTablePath))
                {
                    _errorBuilder.AppendLine("Custom rotation system not found.");
                    return;
                }

                customSystem = JsonConvert.DeserializeObject<RotationSystem>(File.ReadAllText(kickTablePath), StaticSettings.JsonSerializerSettings);
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

            _inputProcessor.SpinHandler = new SpinHandler(_gameSettings.Rules.Controls.ActiveRotationSystem, _gameSettings.Rules.General.AllowedSpins);
        }
    }
}