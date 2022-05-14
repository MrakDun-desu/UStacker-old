using System;
using Blockstacker.GlobalSettings.Changers;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings
{
    public class AppSettingsExporter : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private TMP_InputField _pathField;
        [SerializeField] private GameObject _errorSignal;
        [SerializeField] private TMP_Text _errorText;
        [SerializeField] private GameObject _successSignal;
        [SerializeField] private TMP_Text _successText;

        public event Action SettingChanged;

        public void Export()
        {
            var path = _pathField.text;
            _errorSignal.SetActive(false);
            _successSignal.SetActive(false);
            if (AppSettings.TrySave(path)) {
                _successSignal.SetActive(true);
                _successText.text = "Successfully saved!";
                return;
            }

            _errorSignal.SetActive(true);
            _errorText.text = "Couldn't find save path!";
        }

        public void Import()
        {
            var path = _pathField.text;
            _errorSignal.SetActive(false);
            _successSignal.SetActive(false);
            if (AppSettings.TryLoad(path)) {
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