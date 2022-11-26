using System.Globalization;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Groups;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class GameSettingsOverrideChanger : MonoBehaviour
    {
        [SerializeField] private StringReferenceSO _changedOverride;
        [SerializeField] private TMP_InputField _countdownIntervalField;
        [SerializeField] private TMP_InputField _countdownCountField;
        [SerializeField] private TMP_InputField _startingLevelField;
        [SerializeField] private MusicOptionChanger _musicOptionChanger;
        [SerializeField] private StatCountingGroupChanger _statCountingGroupChanger;

        private GameSettingsOverrides _value;

        private void Awake()
        {
            _countdownIntervalField.onEndEdit.AddListener(OnCountdownIntervalChange);
            _countdownCountField.onEndEdit.AddListener(OnCountdownCountChange);
            _startingLevelField.onEndEdit.AddListener(OnStartingLevelChange);
            _musicOptionChanger.gameTypeStr = _changedOverride;
            _statCountingGroupChanger.gameTypeStr = _changedOverride;
            
            if (!AppSettings.GameOverrides.TryGetValue(_changedOverride.Value, out _value))
            {
                _value = new GameSettingsOverrides();
                AppSettings.GameOverrides[_changedOverride.Value] = _value;
            }

            _countdownIntervalField.text = _value.CountdownInterval.ToString(CultureInfo.InvariantCulture);
            _countdownCountField.text = _value.CountdownCount.ToString(CultureInfo.InvariantCulture);
            _startingLevelField.text = _value.StartingLevel?.ToString(CultureInfo.InvariantCulture);
        }

        private void OnCountdownIntervalChange(string newValue)
        {
            newValue = newValue.Replace('.', ',');
            if (float.TryParse(newValue, out var newInterval))
                _value.CountdownInterval = newInterval;
            
            _countdownIntervalField.text = _value.CountdownInterval.ToString(CultureInfo.InvariantCulture);
        }
        
        private void OnCountdownCountChange(string newValue)
        {
            if (uint.TryParse(newValue, out var newCount))
                _value.CountdownCount = newCount;
            
            _countdownCountField.text = _value.CountdownCount.ToString(CultureInfo.InvariantCulture);
        }
        
        private void OnStartingLevelChange(string newValue)
        {
            _value.StartingLevel = newValue;
            _startingLevelField.text = _value.StartingLevel.ToString(CultureInfo.InvariantCulture);
        }
    }
}