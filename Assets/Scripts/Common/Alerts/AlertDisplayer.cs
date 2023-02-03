using UnityEngine;

namespace UStacker.Common.Alerts
{
    public class AlertDisplayer : MonoSingleton<AlertDisplayer>
    {
        [SerializeField] private AlertController _alertPrefab;
        [SerializeField] private RectTransform _alertsParent;

        public static AlertDisplayer Instance => _instance;

        public void ShowAlert(Alert alert, bool log = true)
        {
            var newAlert = Instantiate(_alertPrefab, _alertsParent);
            newAlert.Initialize(alert);
            if (log)
                Logger.LogAlert(alert);
        }

        [ContextMenu("Show example alert")]
        public void ShowExample()
        {
            ShowAlert(new Alert("Example", "This is an example alert", AlertType.Info), false);
        }
    }
}