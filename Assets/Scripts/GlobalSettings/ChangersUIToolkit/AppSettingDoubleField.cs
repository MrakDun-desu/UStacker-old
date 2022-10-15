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
    public class AppSettingDoubleField : DoubleField, INotifyOnChange
    {
        public new class UxmlTraits : DoubleField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _settingPath = new() {name = "setting-path"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AppSettingDoubleField field) return;

                field.SettingPath = _settingPath.GetValueFromBag(bag, cc);
                field.tabIndex = Mathf.Max(field.tabIndex, 0);
            }
        }

        public new class UxmlFactory : UxmlFactory<AppSettingDoubleField, UxmlTraits>
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

                SetValueWithoutNotify(AppSettings.GetValue<double>(_splitPath));
            }
        }

        private double MinValue { get; set; }
        private double MaxValue { get; set; }
        private bool _validateMin;
        private bool _validateMax;

        private string _settingPath = "";
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);

        public AppSettingDoubleField()
        {
            AddToClassList(StaticChangerData.SETTING_CHANGER_CLASS);
            this.RegisterValueChangedCallback(OnValueChanged);
            AppSettings.SettingsReloaded += () => SetValueWithoutNotify(AppSettings.GetValue<double>(_splitPath));
        }

        private void OnValueChanged(ChangeEvent<double> evt)
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