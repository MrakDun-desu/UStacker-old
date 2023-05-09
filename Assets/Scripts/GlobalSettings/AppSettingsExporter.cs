
/************************************
AppSettingsExporter.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.GlobalSettings.Changers;

namespace UStacker.GlobalSettings
{
    public class AppSettingsExporter : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private TMP_InputField _pathField;
        [SerializeField] private GameObject _errorSignal;
        [SerializeField] private TMP_Text _errorText;
        [SerializeField] private GameObject _successSignal;
        [SerializeField] private TMP_Text _successText;

        [SerializeField] private Button _exportButton;
        [SerializeField] private Button _importButton;

        private void Awake()
        {
            _exportButton.onClick.AddListener(ExportAsync);
            _importButton.onClick.AddListener(ImportAsync);
        }

        public event Action SettingChanged;

        private async void ExportAsync()
        {
            var path = _pathField.text;
            _errorSignal.SetActive(false);
            _successSignal.SetActive(false);
            if (await AppSettings.TrySaveAsync(path))
            {
                _successSignal.SetActive(true);
                _successText.text = "Successfully saved!";
                return;
            }

            _errorSignal.SetActive(true);
            _errorText.text = "Couldn't find save path!";
        }

        private async void ImportAsync()
        {
            var path = _pathField.text;
            _errorSignal.SetActive(false);
            _successSignal.SetActive(false);
            if (await AppSettings.TryLoadAsync(path))
            {
                _successSignal.SetActive(true);
                _successText.text = "Successfully loaded settings!";
                SettingChanged?.Invoke();
                return;
            }

            _errorSignal.SetActive(true);
            _errorText.text = "Couldn't find specified file!";
        }
    }
}
/************************************
end AppSettingsExporter.cs
*************************************/
