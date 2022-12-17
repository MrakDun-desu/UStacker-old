using System.Threading.Tasks;
using UnityEngine;

namespace UStacker.Common.Alerts
{
    public class AlertDisplayer : MonoSingleton<AlertDisplayer>
    {
        [SerializeField] private AlertController _alertPrefab;
        [SerializeField] private RectTransform _alertsParent;

        public static AlertDisplayer Instance => _instance;

        public Task ShowAlert(Alert alert)
        {
            var newAlert = Instantiate(_alertPrefab, _alertsParent);
            newAlert.Initialize(alert);
            return Task.CompletedTask;
        }

        [ContextMenu("Show example alert")]
        public void ShowExample()
        {
            var newAlert = Instantiate(_alertPrefab, _alertsParent);
            newAlert.Initialize(new Alert("Example", "This is an example alert", AlertType.Info));
        }
    }
}