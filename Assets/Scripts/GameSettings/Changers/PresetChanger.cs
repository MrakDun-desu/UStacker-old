using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common.Alerts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GameSettings.Changers
{
    public class PresetChanger : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private string _defaultPrompt = "Pick a preset";
        [SerializeField] private string _emptyPrompt = "No preset available";
        [SerializeField] private Button _savePresetButton;
        [SerializeField] private GameSettingsSO _targetSo;

        private List<string> _availableOptions;

        private void Start()
        {
            RefreshNames();

            _dropdown.onValueChanged.AddListener(OnOptionPicked);
            _savePresetButton.onClick.AddListener(OnSaveButtonClicked);
        }

        private void RefreshNames(string setPresetName = null)
        {
            _dropdown.ClearOptions();

            _availableOptions = GameSettingsSO.EnumeratePresets().ToList();

            _dropdown.options.Add(new TMP_Dropdown.OptionData(_defaultPrompt));
            if (setPresetName is not null)
            {
                for (var i = 0; i < _availableOptions.Count; i++)
                {
                    _dropdown.options.Add(new TMP_Dropdown.OptionData(_availableOptions[i]));
                    if (_availableOptions[i].Equals(setPresetName))
                        _dropdown.SetValueWithoutNotify(i);
                }
            }
            else
                _dropdown.AddOptions(_availableOptions.Select(opt => new TMP_Dropdown.OptionData(opt)).ToList());

            if (_dropdown.options.Count == 1)
            {
                _dropdown.ClearOptions();
                _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
            }

            _dropdown.RefreshShownValue();
        }

        private void OnOptionPicked(int optIndex)
        {
            var presetName = _availableOptions[optIndex];

            if (presetName == _defaultPrompt || presetName == _emptyPrompt)
                return;
            
            if (_targetSo.TryLoad(presetName))
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Game settings reloaded!",
                        $"Game settings overriden with a preset {presetName}.",
                        AlertType.Success));
            }
            else
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Game settings load failed!",
                        $"Game preset {presetName} couldn't be found.",
                        AlertType.Error));
            }
        }

        private void OnSaveButtonClicked()
        {
            var presetName = _targetSo.Presentation.Title;
            presetName = _targetSo.Save(presetName);
            _ = AlertDisplayer.Instance.ShowAlert(
                new Alert("Game settings saved!",
                    $"Game settings have been saved to a preset named {presetName}.",
                    AlertType.Success));

            RefreshNames(presetName);
        }
    }
}