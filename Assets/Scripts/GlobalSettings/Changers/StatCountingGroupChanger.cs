﻿using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class StatCountingGroupChanger : AppSettingChangerBase<Dictionary<string, Guid>>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private StringReferenceSO _gameTypeStr;

        private string _gameType => _gameTypeStr.Value;
        private List<KeyValuePair<Guid, string>> _availableGroups;

        private Guid ChangedGroupId
        {
            get => AppSettings.StatCounting.GameStatCounterDictionary.TryGetValue(_gameType, out var value)
                ? value
                : new Guid();
            set => AppSettings.StatCounting.GameStatCounterDictionary[_gameType] = value;
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
            AppSettings.TrySave();
        }
    }
}