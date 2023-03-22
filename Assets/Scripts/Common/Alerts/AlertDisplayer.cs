using UnityEngine;
using UnityEngine.Pool;

namespace UStacker.Common.Alerts
{
    public class AlertDisplayer : MonoSingleton<AlertDisplayer>
    {
        [SerializeField] private AlertController _alertPrefab;
        [SerializeField] private RectTransform _alertsParent;

        private ObjectPool<AlertController> _alertsPool;

        private AlertController CreateAlert()
        {
            var newAlert = Instantiate(_alertPrefab, _alertsParent);
            newAlert.SourcePool = _alertsPool;
            return newAlert;
        }
        
        protected override void Awake()
        {
            base.Awake();
            _alertsPool = new ObjectPool<AlertController>(
                CreateAlert,
                alert => alert.gameObject.SetActive(true),
                alert => alert.gameObject.SetActive(false),
                alert => Destroy(alert.gameObject));
        }

        private void OnDestroy()
        {
            _alertsPool.Dispose();
        }

        public static void ShowAlert(Alert alert, bool log = true)
        {
            _instance.ShowAlertInner(alert, log);
        }

        private void ShowAlertInner(Alert alert, bool log = true)
        {
            var newAlert = _alertsPool.Get();
            newAlert.Initialize(alert);
            if (log)
                Logger.LogAlert(alert);
        }

        [ContextMenu("Show example alert")]
        public void ShowExample()
        {
            ShowAlert(new Alert("Example", "This is an example alert", AlertType.Info), false);
        }
        
        [ContextMenu("Show example alert with long text")]
        public void ShowLongExample()
        {
            ShowAlert(new Alert("Example", "hello there, my dear lad. How are you doing today? I've been trying to ask you about your business. I sure hope you've been doing well and that no harm has came to you, as it would be quite unfortunate if it did", AlertType.Info), false);
        }
    }
}