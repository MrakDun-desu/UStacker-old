using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Blockstacker.Common.Attributes;
using Blockstacker.Common.Extensions;
using Blockstacker.Common.UIToolkit;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public abstract class AppSettingToggleGroup<T> : BaseField<T>, INotifyOnChange where T : Enum
    {
        public new class UxmlTraits : BaseField<T>.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _settingPath = new() {name = "setting-path"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AppSettingToggleGroup<T> field) return;

                field.SettingPath = _settingPath.GetValueFromBag(bag, cc);
                field.tabIndex = Mathf.Max(field.tabIndex, 0);
            }
        }

        public event Action Changed;

        // if made private will not show up in UIBuilder
        // ReSharper disable once MemberCanBePrivate.Global
        protected string SettingPath
        {
            [UsedImplicitly] get => _settingPath;
            set
            {
                _settingPath = value;
                if (!AppSettings.SettingExists<T>(_splitPath))
                    label = "SETTING NOT FOUND";

                if (AppSettings.TryGetSettingAttribute<TooltipAttribute>(_splitPath, out var tooltipAttr))
                {
                    tooltip = tooltipAttr.tooltip;
                }

                label = AppSettings.TryGetSettingAttribute<DescriptionAttribute>(_splitPath, out var descAttr)
                    ? descAttr.Description
                    : _splitPath[^1].FormatCamelCase();

                SetValueWithoutNotify(AppSettings.GetValue<T>(_splitPath));
            }
        }

        private readonly List<ToggleWithValue> _toggles = new();
        private string _settingPath = "";
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);

        private const string SELF_CLASS = "toggle-group";

        protected AppSettingToggleGroup() : base(string.Empty, new VisualElement())
        {
            AddToClassList(SELF_CLASS);
            var enumType = typeof(T);
            foreach (var enumName in Enum.GetNames(enumType))
            {
                if (enumType.GetMember(enumName)[0].GetCustomAttribute<IgnoreInUIAttribute>(false) is not null)
                    continue;
                
                var enumValue = Enum.Parse<T>(enumName);
                var description = enumType.GetMember(enumName)[0]
                    .GetCustomAttribute<DescriptionAttribute>(false)?.Description;

                if (string.IsNullOrEmpty(description))
                    description = enumName.FormatCamelCase();

                var toggleTooltip = enumType.GetMember(enumName)[0]
                    .GetCustomAttribute<TooltipAttribute>(false)?.tooltip;

                var newToggle = new Toggle(description) {tooltip = toggleTooltip};

                Add(newToggle);
                newToggle.RegisterValueChangedCallback(evt => OnToggleClicked(evt, enumValue));
                _toggles.Add(new ToggleWithValue(newToggle, enumValue));
            }

            this.RegisterValueChangedCallback(OnValueChanged);
        }

        private void OnToggleClicked(ChangeEvent<bool> evt, T toggleValue)
        {
            var toggleValAsLong = Convert.ToInt64(toggleValue);
            if (toggleValAsLong == 0L)
            {
                value = toggleValue;
                (evt.target as Toggle)?.SetValueWithoutNotify(true);
                return;
            }

            value = evt.newValue
                ? (T) Convert.ChangeType(Convert.ToInt64(value) | toggleValAsLong, Enum.GetUnderlyingType(typeof(T)))
                : (T) Convert.ChangeType(Convert.ToInt64(value) & ~toggleValAsLong, Enum.GetUnderlyingType(typeof(T)));
        }

        public override void SetValueWithoutNotify(T newValue)
        {
            base.SetValueWithoutNotify(newValue);
            foreach (var toggle in _toggles)
            {
                var toggleValAsLong = Convert.ToInt64(toggle.Value);
                var newValAsLong = Convert.ToInt64(newValue);
                if (toggleValAsLong == 0L)
                    toggle.Toggle.SetValueWithoutNotify(newValAsLong == toggleValAsLong);
                else 
                    toggle.Toggle.SetValueWithoutNotify((newValAsLong & toggleValAsLong) == toggleValAsLong);
            }
        }

        private void OnValueChanged(ChangeEvent<T> evt)
        {
            AppSettings.TrySetValue(evt.newValue, _splitPath);
            Changed?.Invoke();
        }

        private class ToggleWithValue
        {
            public ToggleWithValue(Toggle toggle, T value)
            {
                Toggle = toggle;
                Value = value;
            }

            public Toggle Toggle { get; }
            public T Value { get; }
        }
    }
}