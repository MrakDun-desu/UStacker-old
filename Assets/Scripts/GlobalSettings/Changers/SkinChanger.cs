using System;
using System.IO;
using UStacker.Common;
using UStacker.GlobalSettings.BlockSkins;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.GlobalSettings.Changers
{
    public class SkinChanger : AppSettingChangerBase<string>
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

            _dropdown.onValueChanged.AddListener(OptionPicked);
            _folderButton.onClick.AddListener(OpenSkinFolder);
            _docsButton.onClick.AddListener(OpenDocumentation);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) return;
            
            RefreshNames();
        }

        private void OptionPicked(int value)
        {
            var newSkin = _dropdown.options[value].text;

            SetValue(newSkin);
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

        private void OpenSkinFolder()
        {
            if (!Directory.Exists(PersistentPaths.Skins))
                Directory.CreateDirectory(PersistentPaths.Skins);

            var defaultPath = Path.Combine(PersistentPaths.Skins, SkinLoader.DEFAULT_PATH);
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            DefaultAppOpener.OpenFile(PersistentPaths.Skins);
        }

        private void OpenDocumentation()
        {
            const string skinDocsUrl = StaticSettings.WikiUrl + "blob/main/Style customization/Skin-customization.md";
            Application.OpenURL(skinDocsUrl);
        }

        private void RefreshNames()
        {
            _dropdown.ClearOptions();
            _dropdown.SetValueWithoutNotify(0);
            foreach (var path in SkinLoader.EnumerateSkins())
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));

            if (!_dropdown.options.Exists(item => item.text == SkinLoader.DEFAULT_PATH))
                _dropdown.options.Insert(0, new TMP_Dropdown.OptionData(SkinLoader.DEFAULT_PATH));

            RefreshValue();
        }
    }
}