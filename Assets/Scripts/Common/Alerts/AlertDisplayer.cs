using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.Common.Alerts
{
    [RequireComponent(typeof(UIDocument))]
    public class AlertDisplayer : MonoSingleton<AlertDisplayer>
    {
        [SerializeField] private float _alertVisibleInterval = 10;
        public static AlertDisplayer Instance => _instance;

        private VisualElement _alertField;
        private const string ALERTS_PARENT_ID = "alertField";
        private const string ALERTS_ADDED_CLASS = "added";
        private const string ALERTS_REMOVED_CLASS = "removed";

        protected override void Awake()
        {
            base.Awake();
            _alertField = GetComponent<UIDocument>().rootVisualElement.Q(ALERTS_PARENT_ID);
        }

        public async Task ShowAlert(Alert alert)
        {
            var newAlert = new AlertUiControl(alert);
            newAlert.AddToClassList(ALERTS_ADDED_CLASS);
            _alertField.hierarchy.Add(newAlert);
            await Task.Delay(500);
            newAlert.RemoveFromClassList(ALERTS_ADDED_CLASS);
            newAlert.RegisterCallback<TransitionEndEvent>(StartAlertCoroutine);
        }

        private IEnumerator RemoveAlertCor(VisualElement alert)
        {
            alert.UnregisterCallback<TransitionEndEvent>(StartAlertCoroutine);
            yield return new WaitForSeconds(_alertVisibleInterval);
            alert.AddToClassList(ALERTS_REMOVED_CLASS);
            alert.RegisterCallback<TransitionEndEvent>(_ => _alertField.hierarchy.Remove(alert));
        }

        private void StartAlertCoroutine(TransitionEndEvent evt)
        {
            StartCoroutine(RemoveAlertCor(evt.currentTarget as VisualElement));
        }
    }
}