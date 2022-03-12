using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Blockstacker.Settings.Changers
{
    public class InputPresetManager : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private TMP_Dropdown _presetPickerDropdown;
        [SerializeField] private TMP_InputField _newPresetNameField;
        [SerializeField] private GameObject _invalidNameSignal;
        [SerializeField] private GameObject _newPresetBlocker;
        [SerializeField] private RebindsPreset[] _premadePresets = new RebindsPreset[0];

        public event Action SettingChanged;
        public static event Action RebindsChanged;

        private static string PresetPath => Path.Combine(Application.persistentDataPath, "InputPresets");


        private void Start()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (_presetPickerDropdown == null) return;

            _presetPickerDropdown.ClearOptions();
            var optionNames = GetAvailablePresets();
            foreach (var optionName in optionNames.Distinct()) {
                _presetPickerDropdown.options.Add(new TMP_Dropdown.OptionData(optionName));
            }
            _presetPickerDropdown.RefreshShownValue();
        }

        private IEnumerable<string> GetAvailablePresets()
        {
            var filenames = new List<string>();
            if (Directory.Exists(PresetPath))
                filenames.AddRange(Directory.EnumerateFiles(PresetPath));
            foreach (var preset in _premadePresets) {
                filenames.Add(preset.Name);
            }
            foreach (var filename in filenames) {
                var presetName = filename;
                if (filename.Contains(".") && filename.Contains("/"))
                    presetName = ExtractOptionName(presetName);
                yield return presetName;
            }
        }

        private string ExtractOptionName(string filename)
        {
            var dotIndex = filename.LastIndexOf(".");
            var slashIndex = filename.LastIndexOf("/") + 1;
            return filename[slashIndex..dotIndex];
        }

        private string WrapOptionName(string optionName) => Path.Combine(PresetPath, $"{optionName}.json");

        private bool IsNameValid(string presetName)
        {
            var isEmpty = string.IsNullOrEmpty(presetName) || presetName.Length <= 1;
            var isInvalidFilename = name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
            return !isEmpty && !isInvalidFilename;
        }

        public void PresetPicked(string optionName)
        {
            var filename = WrapOptionName(optionName);
            if (File.Exists(filename)) {
                AppSettings.Rebinds = File.ReadAllText(filename);
                SettingChanged?.Invoke();
                RebindsChanged?.Invoke();
                return;
            }

            foreach (var preset in _premadePresets) {
                if (preset.Name.Equals(optionName)) {
                    AppSettings.Rebinds = preset.Content;
                    SettingChanged?.Invoke();
                    RebindsChanged?.Invoke();
                    return;
                }
            }
        }

        public void StartSavingPreset() => _newPresetBlocker.SetActive(true);

        public void SavePreset()
        {
            if (!IsNameValid(_newPresetNameField.text)) return;

            var newFile = Path.Combine(PresetPath, $"{_newPresetNameField.text}.json");
            File.WriteAllText(newFile, AppSettings.Rebinds);
            OnValidate();
            ClosePresetBlocker();
        }

        public void ClosePresetBlocker() => _newPresetBlocker.SetActive(false);

        public void ValidateName(string presetName)
        {
            if (IsNameValid(presetName)) {
                _invalidNameSignal.SetActive(true);
            }
            else {
                _invalidNameSignal.SetActive(false);
            }
        }

        public void RevertToDefault()
        {
            AppSettings.Rebinds = "";
            SettingChanged?.Invoke();
            RebindsChanged?.Invoke();
        }

    }
}