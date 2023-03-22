using System;
using System.Collections.Generic;
using UStacker.Common;
using UStacker.GlobalSettings.Music;
using TMPro;
using UnityEngine;

namespace UStacker.GlobalSettings.Changers
{
    public class MusicOptionChanger : AppSettingChangerBase<Dictionary<string, MusicOption>>
    {
        [Space] [SerializeField] private TMP_Dropdown _typeDropdown;
        [SerializeField] private TMP_Dropdown _nameDropdown;
        [SerializeField] private StringReferenceSO _gameTypeStr;

        private readonly List<MusicOption> _groupOptions = new();
        private readonly List<MusicOption> _trackOptions = new();

        public StringReferenceSO GameTypeStr
        {
            get => _gameTypeStr;
            set => _gameTypeStr = value;
        }

        private string _gameType => GameTypeStr.Value;

        private MusicOption ChangedOption
        {
            get => AppSettings.Sound.GameMusicDictionary.TryGetValue(_gameType, out var value) ? value : null;
            set => AppSettings.Sound.GameMusicDictionary[_gameType] = value;
        }

        private void Awake()
        {
            foreach (var option in MusicPlayer.Instance.ListAvailableOptions())
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
        }

        protected override void Start()
        {
            base.Start();
            _nameDropdown.onValueChanged.AddListener(OnNameSelected);
            _typeDropdown.onValueChanged.AddListener(OnTypeSelected);
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

        protected override void RefreshValue()
        {
            _typeDropdown.ClearOptions();
            foreach (var optionName in Enum.GetNames(typeof(OptionType)))
                _typeDropdown.options.Add(new TMP_Dropdown.OptionData(optionName));

            ChangedOption ??= new MusicOption();

            var currentOption = ChangedOption;
            for (var i = 0; i < _typeDropdown.options.Count; i++)
            {
                if (!string.Equals(currentOption.OptionType.ToString(), _typeDropdown.options[i].text)) continue;

                _typeDropdown.SetValueWithoutNotify(i);
                _typeDropdown.RefreshShownValue();
                break;
            }

            RefreshNameOptions(currentOption);
        }

        private void OnTypeSelected(int index)
        {
            if (!Enum.TryParse<OptionType>(_typeDropdown.options[index].text, out var newType))
                return;

            RefreshNameOptions(ChangedOption with
            {
                OptionType = newType
            });

            ChangedOption = new MusicOption(newType, _nameDropdown.options[0].text);
            InvokeSettingChanged();
        }

        private void OnNameSelected(int index)
        {
            var newName = _nameDropdown.options[index].text;
            var optionType = ChangedOption.OptionType;

            ChangedOption = new MusicOption(optionType, newName);

            InvokeSettingChanged();
        }
    }
}