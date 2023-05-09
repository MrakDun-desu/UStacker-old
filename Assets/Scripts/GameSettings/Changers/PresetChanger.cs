
/************************************
PresetChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common;
using UStacker.Common.Alerts;

namespace UStacker.GameSettings.Changers
{
    public class PresetChanger : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private string _defaultPrompt = "Pick a preset";
        [SerializeField] private string _emptyPrompt = "No preset available";
        [SerializeField] private Button _savePresetButton;
        [SerializeField] private Button _openFolderButton;
        [SerializeField] private GameSettingsSO _targetSo;

        private void Start()
        {
            RefreshNames();

            _dropdown.onValueChanged.AddListener(OnOptionPicked);
            _savePresetButton.onClick.AddListener(OnSaveButtonClicked);
            _openFolderButton.onClick.AddListener(OpenFolder);
        }

        private static void OpenFolder()
        {
            DefaultAppOpener.OpenFile(PersistentPaths.GameSettingsPresets);
        }

        private void RefreshNames(string setPresetName = null)
        {
            _dropdown.ClearOptions();

            var availableOptions = GameSettingsSO.EnumeratePresets();

            _dropdown.options.Add(new TMP_Dropdown.OptionData(_defaultPrompt));
            if (setPresetName is not null)
            {
                var optionsToAdd = availableOptions.ToList();
                for (var i = 0; i < optionsToAdd.Count; i++)
                {
                    _dropdown.options.Add(new TMP_Dropdown.OptionData(optionsToAdd[i]));
                    if (optionsToAdd[i].Equals(setPresetName))
                        _dropdown.SetValueWithoutNotify(i);
                }
            }
            else
            {
                _dropdown.AddOptions(availableOptions.Select(opt => new TMP_Dropdown.OptionData(opt)).ToList());
            }

            if (_dropdown.options.Count == 1)
            {
                _dropdown.ClearOptions();
                _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
            }

            _dropdown.RefreshShownValue();
        }

        private void OnOptionPicked(int optIndex)
        {
            _ = TryLoadSettingsAsync(optIndex);
        }

        private async Task TryLoadSettingsAsync(int optIndex)
        {
            var presetName = _dropdown.options[optIndex].text;

            if (presetName == _defaultPrompt || presetName == _emptyPrompt)
                return;

            var shownAlert = await _targetSo.TryLoad(presetName)
                ? new Alert("Game settings reloaded!",
                    $"Game settings overriden with a preset {presetName}.",
                    AlertType.Success)
                : new Alert("Game settings load failed!",
                    $"Game preset {presetName} couldn't be found.",
                    AlertType.Error);

            AlertDisplayer.ShowAlert(shownAlert);
        }

        private void OnSaveButtonClicked()
        {
            _ = SaveSettingsAsync();
        }

        private async Task SaveSettingsAsync()
        {
            var presetName = _targetSo.Settings.Presentation.Title;
            presetName = await _targetSo.Save(presetName);
            AlertDisplayer.ShowAlert(
                new Alert("Game settings saved!",
                    $"Game settings have been saved to a file {presetName}.",
                    AlertType.Success));

            RefreshNames(presetName);
        }
    }
}
/************************************
end PresetChanger.cs
*************************************/
