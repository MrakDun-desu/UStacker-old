using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.StatCounting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class StatCounterChanger : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _typeDropdown;
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private TMP_InputField _filenameField;
        [SerializeField] private TMP_InputField _posXField;
        [SerializeField] private TMP_InputField _posYField;
        [SerializeField] private TMP_InputField _sizeXField;
        [SerializeField] private TMP_InputField _sizeYField;
        [SerializeField] private TMP_InputField _updateIntervalField;
        [SerializeField] private Button _removeButton;
        [SerializeField] private StatCounterSO[] _premadeCounters = Array.Empty<StatCounterSO>();

        private StatCounterRecord _value;

        public StatCounterRecord Value
        {
            get => _value;
            set
            {
                _value = value;
                RefreshValue();
            }
        }

        public event Action<StatCounterChanger> Removed;

        private void Awake()
        {
            AddListenersToFields();
        }

        private void RefreshValue(bool refreshDropdown = true)
        {
            if (refreshDropdown)
            {
                _typeDropdown.ClearOptions();
                var i = 0;
                foreach (var counterName in _premadeCounters.Select(ct => ct.Value.Name))
                {
                    _typeDropdown.options.Add(new TMP_Dropdown.OptionData(counterName));
                    if (counterName.Equals(Value.Name) ||
                        counterName.Equals("Custom") && Value.Type == StatCounterType.Custom)
                        _typeDropdown.SetValueWithoutNotify(i);
                    i++;
                }

                _typeDropdown.RefreshShownValue();
            }

            var displayCustomFields = Value.Type == StatCounterType.Custom;
            _nameField.gameObject.SetActive(displayCustomFields);
            _filenameField.gameObject.SetActive(displayCustomFields);
            
            _nameField.SetTextWithoutNotify(_value.Name);
            _filenameField.SetTextWithoutNotify(_value.Filename);
            _posXField.SetTextWithoutNotify(_value.Position.x.ToString(CultureInfo.InvariantCulture));
            _posYField.SetTextWithoutNotify(_value.Position.y.ToString(CultureInfo.InvariantCulture));
            _sizeXField.SetTextWithoutNotify(_value.Size.x.ToString(CultureInfo.InvariantCulture));
            _sizeYField.SetTextWithoutNotify(_value.Size.y.ToString(CultureInfo.InvariantCulture));
            
            _updateIntervalField.SetTextWithoutNotify(_value.UpdateInterval.ToString(CultureInfo.InvariantCulture));
        }

        private void AddListenersToFields()
        {
            _typeDropdown.onValueChanged.AddListener(OnTypePicked);
            _nameField.onValueChanged.AddListener(OnNameChanged);
            _filenameField.onValueChanged.AddListener(OnFilenameChanged);
            _posXField.onValueChanged.AddListener(OnPosXChanged);
            _posYField.onValueChanged.AddListener(OnPosYChanged);
            _sizeXField.onValueChanged.AddListener(OnSizeXChanged);
            _sizeYField.onValueChanged.AddListener(OnSizeYChanged);
            _updateIntervalField.onValueChanged.AddListener(OnUpdateIntervalChanged);
            
            _removeButton.onClick.AddListener(() => Removed?.Invoke(this));
        }

        private void OnTypePicked(int newValue)
        {
            var pickedValue = _premadeCounters[newValue].Value;

            Value.Type = pickedValue.Type;
            switch (Value.Type)
            {
                case StatCounterType.Normal:
                    Value.Name = pickedValue.Name;
                    Value.Filename = pickedValue.Filename;
                    Value.Script = pickedValue.Script;
                    Value.Position = pickedValue.Position;
                    Value.Size = pickedValue.Size;
                    Value.UpdateInterval = pickedValue.UpdateInterval;
                    break;
                case StatCounterType.Custom:
                    Value.Name = "";
                    Value.Filename = "";
                    Value.Script = "";
                    Value.Position = new Vector2();
                    Value.Size = new Vector2();
                    Value.UpdateInterval = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            RefreshValue(false);
        }

        private void OnNameChanged(string newValue)
        {
            Value.Name = newValue;
        }
        
        private void OnFilenameChanged(string newValue)
        {
            Value.Filename = newValue;
            if (_value.Type != StatCounterType.Custom) return;

            var scriptFilePath = Path.Combine(CustomizationPaths.StatCounters, Value.Filename);
            if (!File.Exists(scriptFilePath))
                return;

            Value.Script = File.ReadAllText(scriptFilePath);
        }

        private void OnPosXChanged(string newValue)
        {
            var converted = float.Parse(newValue);
            _value.Position.x = converted;
        }

        private void OnPosYChanged(string newValue)
        {
            var converted = float.Parse(newValue);
            _value.Position.y = converted;
        }
        
        private void OnSizeXChanged(string newValue)
        {
            var converted = float.Parse(newValue);
            _value.Size.x = converted;
        }

        private void OnSizeYChanged(string newValue)
        {
            var converted = float.Parse(newValue);
            _value.Size.y = converted;
        }

        private void OnUpdateIntervalChanged(string newValue)
        {
            var converted = float.Parse(newValue);
            _value.UpdateInterval = Mathf.Max(converted, 0f);
            _updateIntervalField.SetTextWithoutNotify(_value.UpdateInterval.ToString(CultureInfo.InvariantCulture));
        }
    }
}