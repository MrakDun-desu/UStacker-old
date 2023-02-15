using System.Text;
using UStacker.Gameplay.Presentation;
using UStacker.GameSettings;
using TMPro;

namespace UStacker.Gameplay.Initialization
{
    public class PresentationInitializer : InitializerBase
    {
        private readonly GameCountdown _countdown;
        private readonly TMP_Text _title;

        public PresentationInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO.SettingsContainer gameSettings,
            TMP_Text title,
            GameCountdown countdown)
            : base(problemBuilder, gameSettings)
        {
            _title = title;
            _countdown = countdown;
        }

        public override void Execute()
        {
            _title.text = _gameSettings.Presentation.Title;
            _countdown.CountdownInterval = _gameSettings.Presentation.CountdownInterval;
            _countdown.CountdownCount = _gameSettings.Presentation.CountdownCount;
        }
    }
}