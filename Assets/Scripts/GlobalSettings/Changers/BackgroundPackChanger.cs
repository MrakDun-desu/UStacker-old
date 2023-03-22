using System;
using System.IO;
using UStacker.Common;
using UStacker.GlobalSettings.Backgrounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.GlobalSettings.Changers
{
    public class BackgroundPackChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Button _folderButton;
        [SerializeField] private Button _docsButton;

        private void Awake()
        {
            RefreshNames();
        }

        protected override void Start()
        {
            base.Start();

            _dropdown.onValueChanged.AddListener(OnOptionPicked);
            _folderButton.onClick.AddListener(OpenBackgroundFolder);
            _docsButton.onClick.AddListener(OpenDocumentation);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) return;
            
            RefreshNames();
        }

        private void OnOptionPicked(int value)
        {
            var newBackgroundFolder = _dropdown.options[value].text;

            SetValue(newBackgroundFolder);
        }

        protected override void RefreshValue()
        {
            for (var i = 0; i < _dropdown.options.Count; i++)
            {
                if (!_dropdown.options[i].text.Equals(AppSettings.GetValue<string>(_controlPath))) continue;

                _dropdown.SetValueWithoutNotify(i);
                break;
            }
            _dropdown.RefreshShownValue();
        }

        private void OpenBackgroundFolder()
        {
            if (!Directory.Exists(PersistentPaths.BackgroundPacks))
                Directory.CreateDirectory(PersistentPaths.BackgroundPacks);

            var defaultPath = Path.Combine(PersistentPaths.BackgroundPacks, BackgroundPackLoader.DEFAULT_PATH);
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            DefaultAppOpener.OpenFile(PersistentPaths.BackgroundPacks);
        }

        private void OpenDocumentation()
        {
            const string backgroundDocsUrl = StaticSettings.WikiUrl + "blob/main/Style customization/Background-customization.md";
            Application.OpenURL(backgroundDocsUrl);
        }

        private void RefreshNames()
        {
            _dropdown.ClearOptions();
            _dropdown.SetValueWithoutNotify(0);
            foreach (var path in BackgroundPackLoader.EnumerateBackgroundPacks())
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));

            if (!_dropdown.options.Exists(item => item.text == BackgroundPackLoader.DEFAULT_PATH))
                _dropdown.options.Insert(0, new TMP_Dropdown.OptionData(BackgroundPackLoader.DEFAULT_PATH));
            
            RefreshValue();
        }
    }
}