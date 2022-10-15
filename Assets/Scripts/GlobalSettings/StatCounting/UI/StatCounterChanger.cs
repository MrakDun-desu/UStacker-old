using System;
using System.IO;
using System.Linq;
using Blockstacker.Common;
using Blockstacker.Common.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.StatCounting.UI
{
    public class StatCounterChanger : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<StatCounterChanger>
        {
        }

        private const string SELF_CLASS = "stat-counter-changer";
        private const string BUTTON_CONTAINER_CLASS = "button-container";

        private StatCounterRecord _value = new();

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

        private const string DROPDOWN_LABEL = "Type";
        private const string NAME_LABEL = "Name";
        private const string FILENAME_LABEL = "Filename";
        private const string POSITION_LABEL = "Position";
        private const string SIZE_LABEL = "Size";
        private const string UPDATE_INT_LABEL = "Update interval";
        private const string REMOVE_BUTTON_TEXT = "Remove stat counter";

        private readonly BsDropdownField _typeDropdown;
        private readonly TextField _nameField;
        private readonly TextField _filenameField;
        private readonly Vector2Field _positionField;
        private readonly Vector2Field _sizeField;
        private readonly Slider _updateIntervalSlider;
        private readonly Button _removeButton;

        private readonly StatCounterSO[] _premadeCounterTypes;

        public StatCounterChanger() : this(Array.Empty<StatCounterSO>()) {} 
        
        public StatCounterChanger(StatCounterSO[] premadeCounterTypes)
        {
            _premadeCounterTypes = premadeCounterTypes;
            
            _typeDropdown = new BsDropdownField(DROPDOWN_LABEL);
            _nameField = new TextField(NAME_LABEL);
            _filenameField = new TextField(FILENAME_LABEL);

            _positionField = new Vector2Field(POSITION_LABEL);
            _sizeField = new Vector2Field(SIZE_LABEL);

            _updateIntervalSlider = new Slider(UPDATE_INT_LABEL, 0, 5) {showInputField = true};

            var removeButtonContainer = new VisualElement();
            removeButtonContainer.AddToClassList(BUTTON_CONTAINER_CLASS);
            removeButtonContainer.Add(new Button(() => Removed?.Invoke(this)) {text = REMOVE_BUTTON_TEXT});

            AddToClassList(SELF_CLASS);
            Add(_typeDropdown);
            Add(_nameField);
            Add(_filenameField);
            Add(_positionField);
            Add(_sizeField);
            Add(_updateIntervalSlider);
            Add(removeButtonContainer);

            RefreshValue();
            AddListenersToFields();
        }

        private void RefreshValue(bool refreshDropdown = true)
        {
            if (refreshDropdown) {
                _typeDropdown.Choices.Clear();
                foreach (var value in _premadeCounterTypes.Select(value => value.Value.Name))
                {
                    _typeDropdown.Choices.Add(new BsDropdownField.ChoiceWithTooltip(value));
                }
                _typeDropdown.SetValueWithoutNotify(Value.Type == StatCounterType.Normal ? Value.Name : "Custom");
            }
            var displayCustomFields = Value.Type == StatCounterType.Custom ? DisplayStyle.Flex : DisplayStyle.None;
            _nameField.style.display = displayCustomFields;
            _filenameField.style.display = displayCustomFields;
            
            _nameField.SetValueWithoutNotify(Value.Name);
            _filenameField.SetValueWithoutNotify(Value.Filename);
            _positionField.SetValueWithoutNotify(Value.Position);
            _positionField.SetValueWithoutNotify(Value.Size);
            _updateIntervalSlider.SetValueWithoutNotify(Value.UpdateInterval);
        }

        private void AddListenersToFields()
        {
            _typeDropdown.RegisterValueChangedCallback(_ => OnTypePicked());
            _nameField.RegisterValueChangedCallback(evt => OnNameChanged(evt.newValue));
            _filenameField.RegisterValueChangedCallback(evt => OnFilenameChanged(evt.newValue));
            _positionField.RegisterValueChangedCallback(OnPositionChanged);
            _sizeField.RegisterValueChangedCallback(OnSizeChanged);
            _updateIntervalSlider.RegisterValueChangedCallback(evt => OnUpdateIntervalChanged(evt.newValue));
        }

        private void OnTypePicked()
        {
            var pickedValue = _premadeCounterTypes[_typeDropdown.Index].Value;
            Value.Type = pickedValue.Type;
            switch (pickedValue.Type)
            {
                case StatCounterType.Normal:
                {
                    Value.Name = pickedValue.Name;
                    Value.Filename = pickedValue.Filename;
                    Value.Script = pickedValue.Script;
                    Value.Position = pickedValue.Position;
                    Value.Size = pickedValue.Size;
                    Value.UpdateInterval = pickedValue.UpdateInterval;

                    break;
                }
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

        private void OnNameChanged(string newName)
        {
            Value.Name = newName;
        }

        private void OnFilenameChanged(string filename)
        {
            Value.Filename = filename;
            if (_value.Type != StatCounterType.Custom) return;

            var scriptFilePath = Path.Combine(CustomizationPaths.StatCounters, Value.Filename);
            if (!File.Exists(scriptFilePath))
                return;
            Value.Script = File.ReadAllText(scriptFilePath);
        }

        private void OnPositionChanged(ChangeEvent<Vector2> evt)
        {
            _value.Position = evt.newValue;
        }

        private void OnSizeChanged(ChangeEvent<Vector2> evt)
        {
            _value.Size = evt.newValue;
        }

        private void OnUpdateIntervalChanged(float updateInterval)
        {
            _value.UpdateInterval = Mathf.Max(updateInterval, 0);
            _updateIntervalSlider.SetValueWithoutNotify(_value.UpdateInterval);
        }
    }
}