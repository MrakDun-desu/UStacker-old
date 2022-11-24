using System.IO;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Backgrounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class BackgroundPackChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Button _folderButton;
        [SerializeField] private Button _docsButton;

        private void Start()
        {
            RefreshNames();
            
            AppSettings.SettingsReloaded += RefreshValue;
            _dropdown.onValueChanged.AddListener(OnOptionPicked);
            _folderButton.onClick.AddListener(OpenBackgroundFolder);
            _docsButton.onClick.AddListener(OpenDocumentation);
        }

        private void OnOptionPicked(int value)
        {
            var newBackgroundFolder = _dropdown.options[value].text;

            SetValue(newBackgroundFolder);
        }

        private void RefreshValue()
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
            if (!Directory.Exists(CustomizationPaths.BackgroundPacks))
                Directory.CreateDirectory(CustomizationPaths.BackgroundPacks);

            var defaultPath = Path.Combine(CustomizationPaths.BackgroundPacks, BackgroundPackLoader.DEFAULT_PATH);
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);
            
            DefaultAppOpener.OpenFile(CustomizationPaths.BackgroundPacks);
        }

        private void OpenDocumentation()
        {
            const string backgroundDocsUrl = StaticSettings.WikiUrl + "blob/main/Background-customization.md";
            Application.OpenURL(backgroundDocsUrl);
        }

        public void RefreshNames()
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