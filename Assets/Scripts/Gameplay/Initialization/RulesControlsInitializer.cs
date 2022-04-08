using System.IO;
using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class RulesControlsInitializer : InitializerBase
    {
        private KickSystem _srsKickSystem;
        private KickSystem _srsPlusKickSystem;

        private static string KickSystemsPath => Path.Combine(
            Application.persistentDataPath, "kickTables"
        );

        public RulesControlsInitializer(
            StringBuilder errorBuilder,
            GameSettingsSO gameSettings,
            KickSystem srsKickSystem,
            KickSystem srsPlusKickSystem)
            : base(errorBuilder, gameSettings)
        {
            _srsKickSystem = srsKickSystem;
            _srsPlusKickSystem = srsPlusKickSystem;
        }

        public override void Execute()
        {
            KickSystem customSystem = new();
            if (_gameSettings.Rules.Controls.KickTable ==
                KickTableType.Custom) {

                var kickTablePath = Path.Combine(KickSystemsPath,
                 _gameSettings.Rules.General.RandomBagName);
                if (!File.Exists(kickTablePath)) {
                    _errorBuilder.AppendLine("Custom kicktable not found.");
                    return;
                }
                JsonUtility.FromJsonOverwrite(File.ReadAllText(kickTablePath), customSystem);
            }

            _gameSettings.Rules.Controls.ActualKickTable =
                _gameSettings.Rules.Controls.KickTable switch
                {
                    KickTableType.SRS => _srsKickSystem,
                    KickTableType.SRSPlus => _srsPlusKickSystem,
                    KickTableType.None => new(),
                    KickTableType.Custom => customSystem,
                    _ => new()
                };

        }
    }
}
