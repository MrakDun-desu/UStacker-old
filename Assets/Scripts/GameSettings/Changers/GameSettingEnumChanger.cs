using System;
using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingEnumChanger<TEnum> : GameSettingChangerBase<TEnum> where TEnum : Enum
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        [SerializeField] private EnumWithName<TEnum>[] Values;

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++)
            {
                var value = Values[i].Value;
                _dropdown.options.Add(new TMP_Dropdown.OptionData(Values[i].Name));
                if (value.ToString() == _gameSettingsSO.GetValue<TEnum>(_controlPath).ToString())
                    _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(Values[index].Value);
        }

        [Serializable]
        private struct EnumWithName<T> where T : Enum
        {
            public T Value;
            public string Name;

            public EnumWithName(T value, string name)
            {
                Value = value;
                Name = name;
            }
        }
    }
}