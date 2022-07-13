using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.Music;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class MusicOptionChanger : AppSettingChangerBase<MusicOption>
    {
        [Space]
        [SerializeField] private TMP_Dropdown _typeDropdown;
        [SerializeField] private TMP_Dropdown _nameDropdown;

        private readonly List<MusicOption> _groupOptions = new();
        private readonly List<MusicOption> _trackOptions = new();

        private void Start()
        {
            foreach (var option in MusicPlayer.ListAvailableOptions())
            {
                switch (option.OptionType)
                {
                    case OptionType.Group:
                        _groupOptions.Add(option);
                        break;
                    case OptionType.Track:
                        _trackOptions.Add(option);
                        break;
                    case OptionType.Random:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var currentOption = AppSettings.GetValue<MusicOption>(_controlPath);
            
            _typeDropdown.ClearOptions();
            for (var i = 0; i < Enum.GetNames(typeof(OptionType)).Length; i++)
            {
                var optionType = Enum.GetNames(typeof(OptionType))[i];
                _typeDropdown.options.Add(new TMP_Dropdown.OptionData(optionType));
                if (string.Equals(Enum.GetName(typeof(OptionType), currentOption.OptionType), optionType))
                    _typeDropdown.SetValueWithoutNotify(i);
            }
            _typeDropdown.RefreshShownValue();
            
            RefreshNameOptions(currentOption);
        }

        private void RefreshNameOptions(MusicOption currentOption)
        {
            switch (currentOption.OptionType)
            {
                case OptionType.Group:
                    _nameDropdown.gameObject.SetActive(true);
                    _nameDropdown.ClearOptions();
                    for (var i = 0; i < _groupOptions.Count; i++)
                    {
                        var option = _groupOptions[i];
                        _nameDropdown.options.Add(new TMP_Dropdown.OptionData(option.Name));
                        if (option.Name.Equals(currentOption.Name))
                            _nameDropdown.SetValueWithoutNotify(i);
                    }
                    _nameDropdown.RefreshShownValue();
                    break;
                case OptionType.Track:
                    _nameDropdown.gameObject.SetActive(true);
                    _nameDropdown.ClearOptions();
                    for (var i = 0; i < _trackOptions.Count; i++)
                    {
                        var option = _trackOptions[i];
                        _nameDropdown.options.Add(new TMP_Dropdown.OptionData(option.Name));
                        if (option.Name.Equals(currentOption.Name))
                            _nameDropdown.SetValueWithoutNotify(i);
                    }
                    _nameDropdown.RefreshShownValue();
                    break;
                case OptionType.Random:
                    _nameDropdown.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public void OnTypeSelected(int index)
        {
            if (!Enum.TryParse<OptionType>(_typeDropdown.options[index].text, out var newType))
                return;
            
            RefreshNameOptions(AppSettings.Sound.CustomGameMusic with {OptionType = newType});
            
            SetValue(new MusicOption(newType, _nameDropdown.options[0].text));
            AppSettings.TrySave();
        }

        public void OnNameSelected(int index)
        {
            var newName = _nameDropdown.options[index].text;
            SetValue(AppSettings.GetValue<MusicOption>(_controlPath) with {Name = newName});
            AppSettings.TrySave();
        }

    }
}