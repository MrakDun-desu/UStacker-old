using System;
using TMPro;
using UnityEngine;

namespace UStacker.GlobalSettings.Changers
{
    public class AppSettingEnumChanger<TEnum> : AppSettingChangerBase<TEnum> where TEnum : Enum
    {
        [Space]
        [SerializeField]
        private TMP_Dropdown _dropdown;

        [SerializeField] private EnumWithName[] _values;

        private void Awake()
        {
            _dropdown.ClearOptions();
            foreach (var value in _values) 
                _dropdown.options.Add(new TMP_Dropdown.OptionData(value.Name));
        }

        protected override void Start()
        {
            base.Start();
            _dropdown.onValueChanged.AddListener(OnValuePicked);
        }

        protected override void RefreshValue()
        {
            for (var i = 0; i < _values.Length; i++)
            {
                var value = _values[i];
                if (Convert.ToInt64(value.Value) == Convert.ToInt64(AppSettings.GetValue<TEnum>(_controlPath)))
                    _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        private void OnValuePicked(int index)
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