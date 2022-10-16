using System;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.UIToolkit;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class AppSettingsExporter : VisualElement, INotifyOnChange
    {
        public new class UxmlFactory : UxmlFactory<AppSettingsExporter>
        {
        }

        public event Action Changed;

        private readonly TextField _pathField;

        private const string EXPORT_BUTTON_TEXT = "Export";
        private const string IMPORT_BUTTON_TEXT = "Import";

        private const string SELF_CLASS = "app-settings-exporter";
        private const string BUTTON_CONTAINER_CLASS = "button-container";

        public AppSettingsExporter()
        {
            AddToClassList(SELF_CLASS);
            _pathField = new TextField("Export/import file path");

            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList(BUTTON_CONTAINER_CLASS);
            var exportButton = new Button(Export) {text = EXPORT_BUTTON_TEXT};
            var importButton = new Button(Import) {text = IMPORT_BUTTON_TEXT};
            buttonContainer.Add(exportButton);
            buttonContainer.Add(importButton);

            Add(_pathField);
            Add(buttonContainer);
        }

        private void Export()
        {
            var path = _pathField.value;
            if (AlertDisplayer.Instance == null) return;
            if (AppSettings.TrySave(path))
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Successfully exported!",
                    $"Settings have been successfully exported to {path}.",
                    AlertType.Success));
            }

            _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                "Settings couldn't be exported!",
                $"Could not find export path {path}",
                AlertType.Error));
        }

        private void Import()
        {
            var path = _pathField.value;
            if (AlertDisplayer.Instance == null) return;
            if (AppSettings.TryLoad(path))
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Successfully imported!",
                    $"Successfully imported from path {path}",
                    AlertType.Success));
                Changed?.Invoke();
            }
            
            _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                "Settings couldn't be imported!",
                $"Could not find import path {path}",
                AlertType.Error));
        }
    }
}