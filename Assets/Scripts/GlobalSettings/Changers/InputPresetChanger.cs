using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Blockstacker.Common;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class InputPresetChanger : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private TMP_Dropdown _presetPickerDropdown;
        [SerializeField] private TMP_InputField _newPresetNameField;
        [SerializeField] private GameObject _invalidNameSignal;
        [SerializeField] private GameObject _newPresetBlocker;
        [SerializeField] private string _prompt = "Pick a preset...";


        private void Start()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (_presetPickerDropdown == null) return;

            _presetPickerDropdown.ClearOptions();
            _presetPickerDropdown.options.Add(new TMP_Dropdown.OptionData(_prompt));
            var optionNames = GetAvailablePresets();
            foreach (var optionName in optionNames.Distinct())
                _presetPickerDropdown.options.Add(new TMP_Dropdown.OptionData(optionName));
            _presetPickerDropdown.RefreshShownValue();
        }

        public event Action SettingChanged;
        public static event Action RebindsChanged;

        private static IEnumerable<string> GetAvailablePresets()
        {
            var filenames = new List<string>();
            if (Directory.Exists(PersistentPaths.InputPresets))
                filenames.AddRange(Directory.EnumerateFiles(PersistentPaths.InputPresets));

            foreach (var filename in filenames) yield return Path.GetFileNameWithoutExtension(filename);
        }

        private static string WrapOptionName(string optionName)
        {
            return Path.Combine(PersistentPaths.InputPresets, $"{optionName}.json");
        }

        private static bool IsNameValid(string presetName)
        {
            var isEmpty = string.IsNullOrEmpty(presetName) || presetName.Length <= 0;
            var validationPattern = new StringBuilder();
            validationPattern.Append("[");
            foreach (var character in Path.GetInvalidFileNameChars()) validationPattern.Append(character);
            validationPattern.Append("\\\\]");
            var isInvalidFilename = Regex.IsMatch(presetName, validationPattern.ToString());
            return !isEmpty && !isInvalidFilename;
        }

        public void PresetPicked(int value)
        {
            var optionName = _presetPickerDropdown.options[value].text;
            if (optionName.Equals(_prompt)) return;
            var filename = WrapOptionName(optionName);
            if (!File.Exists(filename)) return;

            AppSettings.Rebinds = File.ReadAllText(filename);
            SettingChanged?.Invoke();
            RebindsChanged?.Invoke();
        }

        public void StartSavingPreset()
        {
            _newPresetBlocker.SetActive(true);
        }

        public void SavePreset()
        {
            if (!IsNameValid(_newPresetNameField.text)) return;
            if (!Directory.Exists(PersistentPaths.InputPresets))
                Directory.CreateDirectory(PersistentPaths.InputPresets);

            var newFile = Path.Combine(PersistentPaths.InputPresets, $"{_newPresetNameField.text}.json");
            File.WriteAllText(newFile, AppSettings.Rebinds);
            OnValidate();
            _newPresetBlocker.SetActive(false);
        }

        public void ValidateName(string presetName)
        {
            _invalidNameSignal.SetActive(!IsNameValid(presetName));
        }

        public void RevertToDefault()
        {
            AppSettings.Rebinds = "";
            SettingChanged?.Invoke();
            RebindsChanged?.Invoke();
        }
    }
}