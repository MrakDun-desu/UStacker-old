using System.IO;
using TMPro;
using UnityEngine;

namespace Blockstacker.Settings
{
    public class AppSettingsExporter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _pathField;
        [SerializeField] private GameObject _invalidNameSignal;


        public void Export()
        {
            var path = _pathField.text;
            _invalidNameSignal.SetActive(false);
            if (AppSettings.TrySave(path)) return;

            _invalidNameSignal.SetActive(true);
        }

        public void Import()
        {
            var path = _pathField.text;
            _invalidNameSignal.SetActive(false);
            if (AppSettings.TryLoad(path)) return;

            _invalidNameSignal.SetActive(true);
        }

    }
}