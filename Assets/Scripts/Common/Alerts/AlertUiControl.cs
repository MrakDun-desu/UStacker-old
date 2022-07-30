using System;
using UnityEngine.UIElements;

namespace Blockstacker.Common.Alerts
{
    public class AlertUiControl : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AlertUiControl> {}

        private const string SELF_CLASS = "alert-control";
        private const string TITLE_CLASS = "alert-title";
        private const string TEXT_CLASS = "alert-text";
        
        private const string SUCCESS_CLASS = "alert-success";
        private const string INFO_CLASS = "alert-info";
        private const string WARNING_CLASS = "alert-warning";
        private const string ERROR_CLASS = "alert-error";

        public AlertUiControl() : this(new Alert("Title", "Text", AlertType.Info))
        {
        }
        
        public AlertUiControl(Alert alert)
        {
            AddToClassList(SELF_CLASS);
            var parentElement = new VisualElement();
            parentElement.AddToClassList(ClassByType(alert.AlertType));
            parentElement.name = "parent";
            Add(parentElement);
            
            var titleElement = new Label(alert.Title);
            titleElement.AddToClassList(TITLE_CLASS);
            titleElement.name = "title";
            
            var textElement = new Label(alert.Text);
            textElement.AddToClassList(TEXT_CLASS);
            textElement.name = "text";
            
            parentElement.Add(titleElement);
            parentElement.Add(textElement);
        }

        private static string ClassByType(AlertType type) =>
            type switch
            {
                AlertType.Success => SUCCESS_CLASS,
                AlertType.Info => INFO_CLASS,
                AlertType.Warning => WARNING_CLASS,
                AlertType.Error => ERROR_CLASS,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

    }
}