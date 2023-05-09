
/************************************
Alert.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.Common.Alerts
{
    public record Alert(string Title, string Text, AlertType AlertType)
    {
        public Alert(string title, AlertType alertType) : this(title, string.Empty, alertType)
        {
        }

        public string Text { get; } = Text;
        public string Title { get; } = Title;
        public AlertType AlertType { get; } = AlertType;
    }

    public enum AlertType
    {
        Success,
        Info,
        Warning,
        Error
    }
}
/************************************
end Alert.cs
*************************************/
