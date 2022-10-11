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
    public class AppSettingFloatField : FloatField, INotifyOnChange
    {
        public new class UxmlTraits : FloatField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _settingPath = new() {name = "setting-path"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AppSettingFloatField field) return;

                field.SettingPath = _settingPath.GetValueFromBag(bag, cc);
                field.tabIndex = Mathf.Max(field.tabIndex, 0);
            }
        }

        public new class UxmlFactory : UxmlFactory<AppSettingFloatField, UxmlTraits>
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

                SetValueWithoutNotify(AppSettings.GetValue<float>(_splitPath));
            }
        }

        private float MinValue { get; set; }
        private float MaxValue { get; set; }
        private bool _validateMin;
        private bool _validateMax;

        private string _settingPath = "";
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);

        public AppSettingFloatField()
        {
            this.RegisterValueChangedCallback(OnValueChanged);
            AppSettings.SettingsReloaded += () => SetValueWithoutNotify(AppSettings.GetValue<float>(_splitPath));
        }

        private void OnValueChanged(ChangeEvent<float> evt)
        {
            var actualValue = evt.newValue;
            if (_validateMax)
                actualValue = Math.Min(actualValue, MaxValue);
            if (_validateMin)
                actualValue = Math.Max(actualValue, MinValue);
            AppSettings.TrySetValue(actualValue, _splitPath);
            Changed?.Invoke();
        }
    }
}