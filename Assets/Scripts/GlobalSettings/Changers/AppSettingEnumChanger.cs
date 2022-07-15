using System;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class AppSettingEnumChanger<TEnum> : AppSettingChangerBase<TEnum> where TEnum : Enum
    {
        [Space]
        [SerializeField]
        private TMP_Dropdown _dropdown;

        [SerializeField] private EnumWithName[] _values;
        
        protected virtual void Start()
        {
            _dropdown.ClearOptions();
            foreach (var value in _values)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(value.Name));
            }

            RefreshDropdownValue();
            AppSettings.SettingsReloaded += RefreshDropdownValue;
        }

        private void RefreshDropdownValue()
        {
            for (var i = 0; i < _values.Length; i++)
            {
                var value = _values[i];
                if (Convert.ToInt64(value.Value) == Convert.ToInt64(AppSettings.GetValue<TEnum>(_controlPath)))
                    _dropdown.SetValueWithoutNotify(i);
            }
            
            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(_values[index].Value);
        }

        [Serializable]
        private struct EnumWithName
        {
            public string Name;
            public TEnum Value;

            public EnumWithName(TEnum value, string name)
            {
                Value = value;
                Name = name;
            }
        }

    }
}