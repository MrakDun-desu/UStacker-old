
/************************************
GameSettingCounterGroupChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.StatCounting;

namespace UStacker.GameSettings.Changers
{
    public class GameSettingCounterGroupChanger : GameSettingChangerBase<StatCounterGroup>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        private readonly List<KeyValuePair<Guid, string>> _availableGroups = new();

        protected override void Start()
        {
            base.Start();
            _dropdown.onValueChanged.AddListener(OnDropdownPicked);
        }

        protected override void RefreshValue()
        {
            _availableGroups.Clear();

            _availableGroups.AddRange(AppSettings.StatCounting.StatCounterGroups
                .Select(entry => new KeyValuePair<Guid, string>(entry.Key, entry.Value.Name)));

            _dropdown.ClearOptions();
            foreach (var (_, value) in _availableGroups)
                _dropdown.options.Add(new TMP_Dropdown.OptionData(value));

            _dropdown.RefreshShownValue();
        }

        private void OnDropdownPicked(int pickedIndex)
        {
            var key = _availableGroups[pickedIndex].Key;
            var newGroup = AppSettings.StatCounting.StatCounterGroups.TryGetValue(key, out var group) switch
            {
                true => group.Copy(),
                false => new StatCounterGroup()
            };

            SetValue(newGroup);
        }
    }
}
/************************************
end GameSettingCounterGroupChanger.cs
*************************************/
