using System.IO;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Music;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class SoundPackChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Button _folderButton;
        [SerializeField] private Button _docsButton;

        private void Start()
        {
            RefreshNames();
            
            AppSettings.SettingsReloaded += RefreshValue;
            _dropdown.onValueChanged.AddListener(OptionPicked);
            _folderButton.onClick.AddListener(OpenSoundFolder);
            _docsButton.onClick.AddListener(OpenDocumentation);
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

        private void OptionPicked(int value)
        {
            var newSoundPack = _dropdown.options[value].text;

            SetValue(newSoundPack);
        }
        
        private void OpenSoundFolder()
        {
            if (!Directory.Exists(CustomizationPaths.SoundPacks))
                Directory.CreateDirectory(CustomizationPaths.SoundPacks);

            var defaultPath = Path.Combine(CustomizationPaths.SoundPacks, SoundPackLoader.DEFAULT_PATH);
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
                Directory.CreateDirectory(Path.Combine(defaultPath, CustomizationFilenames.Music));
                Directory.CreateDirectory(Path.Combine(defaultPath, CustomizationFilenames.SoundEffects));
            }
            
            DefaultAppOpener.OpenFile(CustomizationPaths.SoundPacks);
        }

        private void OpenDocumentation()
        {
            const string backgroundDocsUrl = StaticSettings.WikiUrl + "blob/main/Sound-customization.md";
            Application.OpenURL(backgroundDocsUrl);
        }

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(SoundPackLoader.DEFAULT_PATH));
            _dropdown.SetValueWithoutNotify(0);
            foreach (var path in SoundPackLoader.EnumerateSoundPacks())
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));

            RefreshValue();
        }
    }
}