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
        private readonly bool _isReplay;

        public ControlsInitializer(
            StringBuilder errorBuilder,
            GameSettingsSO.SettingsContainer gameSettings,
            RotationSystem srsRotationSystem,
            RotationSystem srsPlusRotationSystem,
            InputProcessor inputProcessor,
            bool isReplay)
            : base(errorBuilder, gameSettings)
        {
            _srsRotationSystem = srsRotationSystem;
            _srsPlusRotationSystem = srsPlusRotationSystem;
            _inputProcessor = inputProcessor;
            _isReplay = isReplay;
        }

        public override void Execute()
        {
            if (_isReplay)
            {
                _inputProcessor.SpinHandler = new SpinHandler(_gameSettings.Controls.ActiveRotationSystem, _gameSettings.General.AllowedSpins);
                return;
            }
            
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