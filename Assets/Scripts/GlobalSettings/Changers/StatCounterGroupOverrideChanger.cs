using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.GlobalSettings.Groups;

namespace UStacker.GlobalSettings.Changers
{
    public class StatCounterGroupOverrideChanger : MonoBehaviour
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private StringReferenceSO _gameTypeStr;
        private readonly List<KeyValuePair<Guid, string>> _availableGroups = new();

        public StringReferenceSO GameTypeStr
        {
            get => _gameTypeStr;
            set => _gameTypeStr = value;
        }

        private string _gameType => GameTypeStr.Value;

        private Guid? ChangedGroupId
        {
            get => AppSettings.GameOverrides.TryGetValue(_gameType, out var overrides) &&
                   overrides.StatCounterGroupId is { } groupId
                ? groupId
                : null;
            set
            {
                if (AppSettings.GameOverrides.TryGetValue(_gameType, out var overrides))
                    overrides.StatCounterGroupId = value;
                else
                {
                    var newOverrides = new GameSettingsOverrides {StatCounterGroupId = value};
                    AppSettings.GameOverrides[_gameType] = newOverrides;
                }
            }
        }

        private void Start()
        {
            RefreshValue();

            _dropdown.onValueChanged.AddListener(OnDropdownPicked);
            _toggle.onValueChanged.AddListener(OnToggleClicked);
            AppSettings.SettingsReloaded += RefreshValue;
        }

        private void OnToggleClicked(bool isOn)
        {
            if (_availableGroups.Count <= _dropdown.value)
                return;
            
            ChangedGroupId = isOn ? _availableGroups[_dropdown.value].Key : null;
        }

        private void RefreshValue()
        {
            _availableGroups.Clear();

            _availableGroups.AddRange(AppSettings.StatCounting.StatCounterGroups
                .Select(entry => new KeyValuePair<Guid, string>(entry.Key, entry.Value.Name)));

            _toggle.SetIsOnWithoutNotify(ChangedGroupId is not null);

            _dropdown.ClearOptions();
            for (var i = 0; i < _availableGroups.Count; i++)
            {
                var (key, value) = _availableGroups[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(value));
                if (key == ChangedGroupId)
                    _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        private void OnDropdownPicked(int pickedIndex)
        {
            ChangedGroupId = _toggle.isOn ? _availableGroups[pickedIndex].Key : null;
        }
    }
}