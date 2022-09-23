using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Blockstacker.Common;
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
        private const string VECTOR_CONTAINER_CLASS = "vector-container";
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
        private const string REMOVE_BUTTON_TEXT = "Remove counter";

        private readonly DropdownField _typeDropdown;
        private readonly TextField _nameField;
        private readonly TextField _filenameField;
        private readonly TextField _positionXField;
        private readonly TextField _positionYField;
        private readonly TextField _sizeXField;
        private readonly TextField _sizeYField;
        private readonly Slider _updateIntervalSlider;
        private readonly Button _removeButton;

        private readonly VisualElement _nameContainer;
        private readonly VisualElement _filenameContainer;

        private readonly StatCounterSO[] _premadeCounterTypes;

        public StatCounterChanger() : this(Array.Empty<StatCounterSO>()) {} 
        
        public StatCounterChanger(StatCounterSO[] premadeCounterTypes)
        {
            _premadeCounterTypes = premadeCounterTypes;
            
            var dropdownContainer = new VisualElement();
            dropdownContainer.Add(new Label(DROPDOWN_LABEL));
            _typeDropdown = new DropdownField();
            dropdownContainer.Add(_typeDropdown);

            _nameContainer = new VisualElement();
            _nameContainer.Add(new Label(NAME_LABEL));
            _nameField = new TextField();
            _nameContainer.Add(_nameField);

            _filenameContainer = new VisualElement();
            _filenameContainer.Add(new Label(FILENAME_LABEL));
            _filenameField = new TextField();
            _filenameContainer.Add(_filenameField);

            var positionContainer = new VisualElement();
            positionContainer.Add(new Label(POSITION_LABEL));
            _positionXField = new TextField();
            _positionYField = new TextField();
            var positionFields = new VisualElement();
            positionFields.AddToClassList(VECTOR_CONTAINER_CLASS);
            positionFields.Add(_positionXField);
            positionFields.Add(_positionYField);
            positionContainer.Add(positionFields);

            var sizeContainer = new VisualElement();
            sizeContainer.Add(new Label(SIZE_LABEL));
            _sizeXField = new TextField();
            _sizeYField = new TextField();
            var sizeFields = new VisualElement();
            sizeFields.AddToClassList(VECTOR_CONTAINER_CLASS);
            sizeFields.Add(_sizeXField);
            sizeFields.Add(_sizeYField);
            sizeContainer.Add(sizeFields);

            var updateIntervalContainer = new VisualElement();
            updateIntervalContainer.Add(new Label(UPDATE_INT_LABEL));
            _updateIntervalSlider = new Slider(0, 5) {showInputField = true};
            updateIntervalContainer.Add(_updateIntervalSlider);

            var removeButtonContainer = new VisualElement();
            removeButtonContainer.AddToClassList(BUTTON_CONTAINER_CLASS);
            removeButtonContainer.Add(new Button(() => Removed?.Invoke(this)) {text = REMOVE_BUTTON_TEXT});

            AddToClassList(SELF_CLASS);
            Add(dropdownContainer);
            Add(_nameContainer);
            Add(_filenameContainer);
            Add(positionContainer);
            Add(sizeContainer);
            Add(updateIntervalContainer);
            Add(removeButtonContainer);

            RefreshValue();
            AddListenersToFields();
        }

        private void RefreshValue(bool refreshDropdown = true)
        {
            if (refreshDropdown) {
                _typeDropdown.choices.Clear();
                _typeDropdown.choices.AddRange(_premadeCounterTypes.Select(value => value.Value.Name));
                _typeDropdown.SetValueWithoutNotify(Value.Type == StatCounterType.Normal ? Value.Name : "Custom");
            }
            var displayCustomFields = Value.Type == StatCounterType.Custom ? DisplayStyle.Flex : DisplayStyle.None;
            _nameContainer.style.display = displayCustomFields;
            _filenameContainer.style.display = displayCustomFields;
            
            _nameField.SetValueWithoutNotify(Value.Name);
            _filenameField.SetValueWithoutNotify(Value.Filename);
            _positionXField.SetValueWithoutNotify(Value.Position.x.ToString(CultureInfo.InvariantCulture));
            _positionYField.SetValueWithoutNotify(Value.Position.y.ToString(CultureInfo.InvariantCulture));
            _sizeXField.SetValueWithoutNotify(Value.Size.x.ToString(CultureInfo.InvariantCulture));
            _sizeYField.SetValueWithoutNotify(Value.Size.y.ToString(CultureInfo.InvariantCulture));
            _updateIntervalSlider.SetValueWithoutNotify(Value.UpdateInterval);
        }

        private void AddListenersToFields()
        {
            _typeDropdown.RegisterValueChangedCallback(_ => OnTypePicked());
            _nameField.RegisterValueChangedCallback(evt => OnNameChanged(evt.newValue));
            _filenameField.RegisterValueChangedCallback(evt => OnFilenameChanged(evt.newValue));
            _positionXField.RegisterValueChangedCallback(evt => OnPositionXChanged(evt.newValue));
            _positionYField.RegisterValueChangedCallback(evt => OnPositionYChanged(evt.newValue));
            _sizeXField.RegisterValueChangedCallback(evt => OnSizeXChanged(evt.newValue));
            _sizeYField.RegisterValueChangedCallback(evt => OnSizeYChanged(evt.newValue));
            _updateIntervalSlider.RegisterValueChangedCallback(evt => OnUpdateIntervalChanged(evt.newValue));
        }

        private void OnTypePicked()
        {
            var pickedValue = _premadeCounterTypes[_typeDropdown.index].Value;
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

        private void OnPositionXChanged(string posXstr)
        {
            if (!float.TryParse(posXstr, out var posX))
            {
                _positionXField.SetValueWithoutNotify(_value.Position.x.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _value.Position.x = posX;
            }
        }

        private void OnPositionYChanged(string posYstr)
        {
            if (!float.TryParse(posYstr, out var posY))
            {
                _positionYField.SetValueWithoutNotify(_value.Position.y.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _value.Position.y = posY;
            }
        }

        private void OnSizeXChanged(string sizeXstr)
        {
            if (!float.TryParse(sizeXstr, out var sizeX))
            {
                _sizeXField.SetValueWithoutNotify(_value.Size.x.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _value.Size.x = sizeX;
            }
        }

        private void OnSizeYChanged(string sizeYstr)
        {
            if (!float.TryParse(sizeYstr, out var sizeY))
            {
                _sizeYField.SetValueWithoutNotify(_value.Size.y.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _value.Size.y = sizeY;
            }
        }

        private void OnUpdateIntervalChanged(float updateInterval)
        {
            _value.UpdateInterval = Mathf.Max(updateInterval, 0);
            _updateIntervalSlider.SetValueWithoutNotify(_value.UpdateInterval);
        }
    }
}