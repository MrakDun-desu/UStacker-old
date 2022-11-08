using System.Text;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.Gameplay.Spins;
using Blockstacker.GlobalSettings;

namespace Blockstacker.Gameplay.Initialization
{
    public class ControlsInitializer : InitializerBase
    {
        private readonly InputProcessor _inputProcessor;
        private readonly RotationSystem _srsPlusRotationSystem;
        private readonly RotationSystem _srsRotationSystem;

        public ControlsInitializer(
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
            _gameSettings.Controls.ActiveRotationSystem =
                _gameSettings.Controls.RotationSystemType switch
                {
                    RotationSystemType.SRS => _srsRotationSystem,
                    RotationSystemType.SRSPlus => _srsPlusRotationSystem,
                    RotationSystemType.None => new RotationSystem(),
                    RotationSystemType.Custom => _gameSettings.Controls.ActiveRotationSystem,
                    _ => new RotationSystem()
                };

            _inputProcessor.SpinHandler = new SpinHandler(_gameSettings.Controls.ActiveRotationSystem, _gameSettings.General.AllowedSpins);
            
            if (!_gameSettings.Controls.OverrideHandling)
                _gameSettings.Controls.Handling = AppSettings.Handling with {};
        }
    }
}