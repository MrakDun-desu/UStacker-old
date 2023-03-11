using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using TMPro;
using UnityEngine;
using UStacker.GlobalSettings.Groups;

namespace UStacker.GlobalSettings.Changers
{
    public class StatCountingGroupChanger : AppSettingChangerBase<Dictionary<string, Guid>>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private StringReferenceSO _gameTypeStr;
        private List<KeyValuePair<Guid, string>> _availableGroups;

        public StringReferenceSO GameTypeStr
        {
            get => _gameTypeStr;
            set => _gameTypeStr = value;
        }

        private string _gameType => GameTypeStr.Value;

        private Guid ChangedGroupId
        {
            get => AppSettings.GameOverrides.TryGetValue(_gameType, out var overrides) && overrides.StatCounterGroupId is {} groupId
                ? groupId
                : Guid.Empty;
            set
            {
                if (AppSettings.GameOverrides.TryGetValue(_gameType, out var overrides))
                    overrides.StatCounterGroupId = value;
                else
                {
                    var newOverrides = new GameSettingsOverrides { StatCounterGroupId = value };
                    AppSettings.GameOverrides[_gameType] = newOverrides;
                }
            }
        }

        private void Start()
        {
            RefreshValue();

            _dropdown.onValueChanged.AddListener(OnDropdownPicked);
            AppSettings.SettingsReloaded += RefreshValue;
        }

        private void RefreshValue()
        {
            _availableGroups = AppSettings.StatCounting.StatCounterGroups
                .Select(entry => new KeyValuePair<Guid, string>(entry.Key, entry.Value.Name))
                .ToList();

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
            ChangedGroupId = _availableGroups[pickedIndex].Key;
            
            InvokeSettingChanged();
        }
    }
}