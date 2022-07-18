using System.Text;
using Blockstacker.Gameplay.Presentation;
using Blockstacker.GameSettings;
using TMPro;
using UnityEngine;

namespace Blockstacker.Gameplay.Initialization
{
    public class PresentationInitializer : InitializerBase
    {
        private readonly GameCountdown _countdown;
        private readonly TMP_Text _title;

        public PresentationInitializer(
            StringBuilder problemBuilder,
            GameSettingsSO gameSettings,
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
            if (_gameSettings.Presentation.UseCountdown)
            {
                _countdown.CountdownInterval = _gameSettings.Presentation.CountdownInterval;
                _countdown.CountdownCount = _gameSettings.Presentation.CountdownCount;
            }
            else
            {
                _countdown.CountdownInterval = .5f;
                _countdown.CountdownCount = 0;
            }
        }
    }
}