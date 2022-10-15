using System;
using System.ComponentModel;
using Blockstacker.Common.Extensions;
using Blockstacker.Common.UIToolkit;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class AppSettingToggle : Toggle, INotifyOnChange
    {
        public new class UxmlTraits : Toggle.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _settingPath = new() {name = "setting-path"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AppSettingToggle toggle) return;

                toggle.SettingPath = _settingPath.GetValueFromBag(bag, cc);
                toggle.tabIndex = Mathf.Max(toggle.tabIndex, 0);
            }
        }

        public new class UxmlFactory : UxmlFactory<AppSettingToggle, UxmlTraits>
        {
        }

        public event Action Changed;

        private string SettingPath
        {
            [UsedImplicitly] get => _settingPath;
            set
            {
                _settingPath = value;
                if (!AppSettings.SettingExists<bool>(_splitPath))
                {
                    label = "SETTING NOT FOUND";
                    return;
                }

                if (AppSettings.TryGetSettingAttribute<TooltipAttribute>(_splitPath, out var tooltipAttr))
                {
                    tooltip = tooltipAttr.tooltip;
                }

                label = AppSettings.TryGetSettingAttribute<DescriptionAttribute>(_splitPath, out var descAttr)
                    ? descAttr.Description
                    : _splitPath[^1].FormatCamelCase();

                SetValueWithoutNotify(AppSettings.GetValue<bool>(_splitPath));
            }
        }

        private string _settingPath = "";
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);

        public AppSettingToggle()
        {
            AddToClassList(StaticChangerData.SETTING_CHANGER_CLASS);
            this.RegisterValueChangedCallback(OnValueChanged);
            AppSettings.SettingsReloaded += () => SetValueWithoutNotify(AppSettings.GetValue<bool>(_splitPath));
        }

        private void OnValueChanged(ChangeEvent<bool> evt)
        {
            AppSettings.TrySetValue(evt.newValue, _splitPath);
            Changed?.Invoke();
        }
    }
}