using System.Globalization;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Groups;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class GameSettingsOverrideChanger : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _countdownIntervalField;
        [SerializeField] private TMP_InputField _countdownCountField;
        [SerializeField] private TMP_InputField _startingLevelField;

        private GameSettingsOverrides _value;
        public StringReferenceSO ChangedOverrideName { get; set; }

        private void Start()
        {
            _countdownIntervalField.onEndEdit.AddListener(OnCountdownIntervalChange);
            _countdownCountField.onEndEdit.AddListener(OnCountdownCountChange);
            _startingLevelField.onEndEdit.AddListener(OnStartingLevelChange);

            if (!AppSettings.GameOverrides.TryGetValue(ChangedOverrideName.Value, out _value))
            {
                _value = new GameSettingsOverrides();
                AppSettings.GameOverrides[ChangedOverrideName.Value] = _value;
            }

            if (_value.CountdownInterval is { } interval)
                _countdownIntervalField.text = interval.ToString(CultureInfo.InvariantCulture);
            if (_value.CountdownCount is { } count)
                _countdownCountField.text = count.ToString(CultureInfo.InvariantCulture);

            _startingLevelField.text = _value.StartingLevel;
        }

        private void OnCountdownIntervalChange(string newValue)
        {
            newValue = newValue.Replace('.', ',');
            if (float.TryParse(newValue, out var newInterval))
                _value.CountdownInterval = newInterval;

            if (_value.CountdownInterval is { } interval)
                _countdownIntervalField.text = interval.ToString(CultureInfo.InvariantCulture);
        }

        private void OnCountdownCountChange(string newValue)
        {
            if (uint.TryParse(newValue, out var newCount))
                _value.CountdownCount = newCount;

            if (_value.CountdownCount is { } count)
                _countdownCountField.text = count.ToString(CultureInfo.InvariantCulture);
        }

        private void OnStartingLevelChange(string newValue)
        {
            _value.StartingLevel = newValue;
            _startingLevelField.text = _value.StartingLevel.ToString(CultureInfo.InvariantCulture);
        }
    }
}