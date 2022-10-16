using System;
using System.ComponentModel;
using Blockstacker.Common.Extensions;
using Blockstacker.Common.UIToolkit;
using Blockstacker.GlobalSettings.Backgrounds;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class BackgroundPackChanger : BsDropdownField, INotifyOnChange
    {
        public new class UxmlTraits : BsDropdownField.UxmlTraits
        {
        }

        public new class UxmlFactory : UxmlFactory<BackgroundPackChanger, UxmlTraits>
        {
        }
        
        private string SettingPath
        {
            [UsedImplicitly] get => _settingPath;
            set
            {
                _settingPath = value;
                if (!AppSettings.SettingExists<string>(_splitPath))
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

                SetValueWithoutNotify(AppSettings.GetValue<string>(_splitPath));
            }
        }

        private string _settingPath = "";
        private string[] _splitPath => _settingPath.Split(AppSettings.NAME_SEPARATOR);
        
        public event Action Changed;

        private const string DEFAULT_PACK_NAME = "default";

        public BackgroundPackChanger() : this(string.Empty)
        {
        }
        
        public BackgroundPackChanger(string settingPath)
        {
            SettingPath = settingPath;
            RefreshNames();
            RefreshValue();

            AppSettings.SettingsReloaded += RefreshValue;
            this.RegisterValueChangedCallback(OnValueChanged);
        }

        public void RefreshNames()
        {
            Choices.Clear();
            Choices.Add(new ChoiceWithTooltip(DEFAULT_PACK_NAME));

            foreach (var path in BackgroundPackLoader.EnumerateBackgroundPacks())
                Choices.Add(new ChoiceWithTooltip(path));
        }

        public void RefreshValue()
        {
            SetValueWithoutNotify(AppSettings.GetValue<string>(_splitPath));
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            var newValue = evt.newValue;
            if (newValue.Equals(DEFAULT_PACK_NAME))
                newValue = string.Empty;

            AppSettings.TrySetValue(newValue, _splitPath);
            Changed?.Invoke();
        }
    }
}