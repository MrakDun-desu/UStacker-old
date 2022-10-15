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
    public class AppSettingDoubleSlider : SliderInt, INotifyOnChange
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _settingPath = new() {name = "setting-path"};
            private readonly UxmlStringAttributeDescription _label = new() {name = "label"};
            private readonly UxmlStringAttributeDescription _unitLabel = new() {name = "unit-label"};
            private readonly UxmlDoubleAttributeDescription _multiplier = new() {name = "multiplier"};
            private readonly UxmlDoubleAttributeDescription _minValue = new() {name = "min-value"};
            private readonly UxmlDoubleAttributeDescription _maxValue = new() {name = "max-value"};
            private readonly UxmlIntAttributeDescription _stepCount = new() {name = "step-count"};
            private readonly UxmlBoolAttributeDescription _showFrameField = new() {name = "show-frame-field"};
            private readonly UxmlBoolAttributeDescription _maxIsInfinity = new() {name = "max-is-infinity"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AppSettingDoubleSlider slider) return;

                slider.label = _label.GetValueFromBag(bag, cc);
                slider.UnitLabel = _unitLabel.GetValueFromBag(bag, cc);
                slider.Multiplier = _multiplier.GetValueFromBag(bag, cc);
                slider.StepCount = _stepCount.GetValueFromBag(bag, cc);
                slider.ShowFrameField = _showFrameField.GetValueFromBag(bag, cc);
                slider.MaxIsInfinity = _maxIsInfinity.GetValueFromBag(bag, cc);
                slider.MinValue = _minValue.GetValueFromBag(bag, cc);
                slider.MaxValue = _maxValue.GetValueFromBag(bag, cc);
                slider.SettingPath = _settingPath.GetValueFromBag(bag, cc);

                slider.tabIndex = Mathf.Max(slider.tabIndex, 0);
            }
        }

        public new class UxmlFactory : UxmlFactory<AppSettingDoubleSlider, UxmlTraits>
        {
        }

        public event Action Changed;

        private string SettingPath
        {
            [UsedImplicitly] get => _settingPath;
            set
            {
                _settingPath = value;
                if (!AppSettings.SettingExists<double>(_splitPath))
                {
                    label = "SETTING NOT FOUND";
                    return;
                }

                if (AppSettings.TryGetSettingAttribute<MinRestraintAttribute>(_splitPath, out var minAttr))
                {
                    MinValue = minAttr.Value;
                    _validateMin = minAttr.UseForValidation;
                }

                if (AppSettings.TryGetSettingAttribute<MaxRestraintAttribute>(_splitPath, out var maxAttr))
                {
                    MaxValue = maxAttr.Value;
                    _validateMax = maxAttr.UseForValidation;
                }

                if (AppSettings.TryGetSettingAttribute<TooltipAttribute>(_splitPath, out var tooltipAttr))
                {
                    tooltip = tooltipAttr.tooltip;
                }

                label = AppSettings.TryGetSettingAttribute<DescriptionAttribute>(_splitPath, out var descAttr)
                    ? descAttr.Description
                    : _splitPath[^1].FormatCamelCase();

                SetRealValue(AppSettings.GetValue<double>(_splitPath));
            }
        }

        private bool ShowFrameField
        {
            [UsedImplicitly] get => _showFrameField;
            set
            {
                _showFrameField = value;
                var frameDisplay = value ? DisplayStyle.Flex : DisplayStyle.None;
                _frameField.style.display = frameDisplay;
                _frameUnitLabel.style.display = frameDisplay;
            }
        }

        private double Multiplier { get; set; }
        private double MinValue { get; set; }
        private double MaxValue { get; set; }
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
        private bool _showFrameField;
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);
        private readonly DoubleField _displayField;
        private readonly DoubleField _frameField;
        private readonly Label _unitLabel;
        private readonly Label _frameUnitLabel;
        private bool _validateMin;
        private bool _validateMax;
        private const double FPS = 60;

        private const string DISPLAY_FIELD_CLASS = "display-field";
        private const string FRAME_FIELD_CLASS = "frame-field";
        private const string UNIT_LABEL_CLASS = "unit-label";

        public AppSettingDoubleSlider()
        {
            AddToClassList(StaticChangerData.SETTING_CHANGER_CLASS);
            
            _displayField = new DoubleField();
            _unitLabel = new Label();
            _frameField = new DoubleField();
            _frameUnitLabel = new Label("F");

            _displayField.AddToClassList(DISPLAY_FIELD_CLASS);
            _unitLabel.AddToClassList(UNIT_LABEL_CLASS);
            _frameField.AddToClassList(FRAME_FIELD_CLASS);
            _frameUnitLabel.AddToClassList(UNIT_LABEL_CLASS);

            Add(_displayField);
            Add(_unitLabel);
            Add(_frameField);
            Add(_frameUnitLabel);
            _displayField.isDelayed = true;
            _frameField.isDelayed = true;

            _displayField.RegisterValueChangedCallback(OnFieldValueChanged);
            _frameField.RegisterValueChangedCallback(OnFrameFieldValueChanged);
            this.RegisterValueChangedCallback(OnSliderValueChanged);
            AppSettings.SettingsReloaded += () => SetRealValue(AppSettings.GetValue<double>(_splitPath));
        }

        private void OnFieldValueChanged(ChangeEvent<double> evt)
        {
            var actualValue = evt.newValue / Multiplier;
            SetRealValue(actualValue);
        }

        private void OnFrameFieldValueChanged(ChangeEvent<double> evt)
        {
            var actualValue = evt.newValue / FPS;
            SetRealValue(actualValue);
        }

        private void OnSliderValueChanged(ChangeEvent<int> evt)
        {
            if (MaxIsInfinity && evt.newValue == highValue)
            {
                SetRealValue(double.PositiveInfinity);
                return;
            }

            var actualValue = (double) evt.newValue / StepCount * (MaxValue - MinValue) + MinValue;
            SetRealValue(actualValue);
        }

        private void SetRealValue(double newValue)
        {
            if (_validateMax)
                newValue = Math.Min(newValue, MaxValue);
            if (_validateMin)
                newValue = Math.Max(newValue, MinValue);

            _displayField.SetValueWithoutNotify(Math.Round(newValue * Multiplier, 2));
            _frameField.SetValueWithoutNotify(Math.Round(newValue * FPS, 2));

            if (!double.IsPositiveInfinity(newValue))
                SetValueWithoutNotify((int) ((newValue - MinValue) * StepCount / (MaxValue - MinValue)));
            else
                SetValueWithoutNotify(highValue);

            AppSettings.TrySetValue(newValue, _splitPath);
            Changed?.Invoke();
        }
    }
}