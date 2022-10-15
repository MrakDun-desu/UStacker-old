using System;
using System.ComponentModel;
using Blockstacker.Common.Attributes;
using Blockstacker.Common.Extensions;
using Blockstacker.Common.UIToolkit;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class AppSettingFloatSlider : SliderInt, INotifyOnChange
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _settingPath = new() {name = "setting-path"};
            private readonly UxmlStringAttributeDescription _label = new() {name = "label"};
            private readonly UxmlStringAttributeDescription _unitLabel = new() {name = "unit-label"};
            private readonly UxmlFloatAttributeDescription _multiplier = new() {name = "multiplier"};
            private readonly UxmlFloatAttributeDescription _minValue = new() {name = "min-value"};
            private readonly UxmlFloatAttributeDescription _maxValue = new() {name = "max-value"};
            private readonly UxmlIntAttributeDescription _stepCount = new() {name = "step-count"};
            private readonly UxmlBoolAttributeDescription _maxIsInfinity = new() {name = "max-is-infinity"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AppSettingFloatSlider slider) return;

                slider.label = _label.GetValueFromBag(bag, cc);
                slider.UnitLabel = _unitLabel.GetValueFromBag(bag, cc);
                slider.Multiplier = _multiplier.GetValueFromBag(bag, cc);
                slider.StepCount = _stepCount.GetValueFromBag(bag, cc);
                slider.MaxIsInfinity = _maxIsInfinity.GetValueFromBag(bag, cc);
                slider.MinValue = _minValue.GetValueFromBag(bag, cc);
                slider.MaxValue = _maxValue.GetValueFromBag(bag, cc);
                slider.SettingPath = _settingPath.GetValueFromBag(bag, cc);

                slider.tabIndex = Mathf.Max(slider.tabIndex, 0);
            }
        }

        public new class UxmlFactory : UxmlFactory<AppSettingFloatSlider, UxmlTraits>
        {
        }

        public event Action Changed;

        private string SettingPath
        {
            [UsedImplicitly] get => _settingPath;
            set
            {
                _settingPath = value;
                if (!AppSettings.SettingExists<float>(_splitPath))
                {
                    label = "SETTING NOT FOUND";
                    return;
                }

                if (AppSettings.TryGetSettingAttribute<MinRestraintAttribute>(_splitPath, out var minAttr))
                {
                    MinValue = (float)minAttr.Value;
                    _validateMin = minAttr.UseForValidation;
                }

                if (AppSettings.TryGetSettingAttribute<MaxRestraintAttribute>(_splitPath, out var maxAttr))
                {
                    MaxValue = (float)maxAttr.Value;
                    _validateMax = maxAttr.UseForValidation;
                }

                if (AppSettings.TryGetSettingAttribute<TooltipAttribute>(_splitPath, out var tooltipAttr))
                {
                    tooltip = tooltipAttr.tooltip;
                }

                label = AppSettings.TryGetSettingAttribute<DescriptionAttribute>(_splitPath, out var descAttr)
                    ? descAttr.Description
                    : _splitPath[^1].FormatCamelCase();

                SetRealValue(AppSettings.GetValue<float>(_splitPath));
            }
        }

        private float Multiplier { get; set; }
        private float MinValue { get; set; }
        private float MaxValue { get; set; }
        private bool MaxIsInfinity { get; set; }

        private int StepCount
        {
            get => highValue;
            set => highValue = value;
        }

        private string UnitLabel
        {
            [UsedImplicitly] get => _unitLabel.text;
            set => _unitLabel.text = value;
        }

        private string _settingPath = "";
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);
        private readonly FloatField _displayField;
        private readonly Label _unitLabel;
        private bool _validateMin;
        private bool _validateMax;

        private const string DISPLAY_FIELD_CLASS = "display-field";
        private const string UNIT_LABEL_CLASS = "unit-label";

        public AppSettingFloatSlider()
        {
            AddToClassList(StaticChangerData.SETTING_CHANGER_CLASS);
            
            _displayField = new FloatField();
            _unitLabel = new Label();

            _displayField.AddToClassList(DISPLAY_FIELD_CLASS);
            _unitLabel.AddToClassList(UNIT_LABEL_CLASS);

            Add(_displayField);
            Add(_unitLabel);
            _displayField.isDelayed = true;

            _displayField.RegisterValueChangedCallback(OnFieldValueChanged);
            this.RegisterValueChangedCallback(OnSliderValueChanged);
            AppSettings.SettingsReloaded += () => SetRealValue(AppSettings.GetValue<float>(_splitPath));
        }

        private void OnFieldValueChanged(ChangeEvent<float> evt)
        {
            var actualValue = evt.newValue / Multiplier;
            SetRealValue(actualValue);
        }

        private void OnSliderValueChanged(ChangeEvent<int> evt)
        {
            if (MaxIsInfinity && evt.newValue == highValue)
            {
                SetRealValue(float.PositiveInfinity);
                return;
            }

            var actualValue = (float) evt.newValue / StepCount * (MaxValue - MinValue) + MinValue;
            SetRealValue(actualValue);
        }

        private void SetRealValue(float newValue)
        {
            if (_validateMax)
                newValue = Math.Min(newValue, MaxValue);
            if (_validateMin)
                newValue = Math.Max(newValue, MinValue);

            _displayField.SetValueWithoutNotify((float)Math.Round(newValue * Multiplier, 2));

            if (!float.IsPositiveInfinity(newValue))
                SetValueWithoutNotify((int) ((newValue - MinValue) * StepCount / (MaxValue - MinValue)));
            else
                SetValueWithoutNotify(highValue);

            AppSettings.TrySetValue(newValue, _splitPath);
            Changed?.Invoke();
        }
    }
}