using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Blockstacker.Common.Extensions;
using Blockstacker.Common.UIToolkit;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public abstract class AppSettingEnumDropdown<T> : BsDropdownField, INotifyOnChange where T : Enum
    {
        public new class UxmlTraits : BsDropdownField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _settingPath = new() {name = "setting-path"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AppSettingEnumDropdown<T> field) return;

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

                var settingValue = AppSettings.GetValue<T>(_splitPath);
                var shownValue = typeof(T).GetMember(settingValue.ToString())[0]
                    .GetCustomAttribute<DescriptionAttribute>(false)
                    ?.Description;
                shownValue ??= settingValue.ToString().FormatCamelCase();

                SetValueWithoutNotify(shownValue);
            }
        }

        private string _settingPath = "";
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);
        private readonly List<T> _values = new();

        private const string SELF_CLASS = "enum-dropdown";

        protected AppSettingEnumDropdown()
        {
            AddToClassList(SELF_CLASS);
            var enumType = typeof(T);

            Choices.Clear();
            foreach (var enumName in Enum.GetNames(enumType))
            {
                var enumValue = Enum.Parse<T>(enumName);
                var description = enumType.GetMember(enumName)[0]
                    .GetCustomAttribute<DescriptionAttribute>(false)?.Description;

                if (string.IsNullOrEmpty(description))
                    description = enumName.FormatCamelCase();

                var choiceTooltip = enumType.GetMember(enumName)[0]
                    .GetCustomAttribute<TooltipAttribute>(false)?.tooltip;

                Choices.Add(new ChoiceWithTooltip(description, choiceTooltip));
                _values.Add(enumValue);
            }

            this.RegisterValueChangedCallback(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<string> _)
        {
            var newValue = _values[Index];
            AppSettings.TrySetValue(newValue, _splitPath);
            Changed?.Invoke();
        }
    }
}