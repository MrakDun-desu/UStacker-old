
/************************************
TooltipTrigger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UStacker.Common.UI
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private const float WAIT_SECONDS = 0.5f;
        [TextArea(5, 30)] [SerializeField] private string _tooltipText = string.Empty;

        private Coroutine _displayCoroutine;

        private void OnDisable()
        {
            HideTooltip();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HideTooltip();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(_tooltipText))
                return;

            _displayCoroutine = StartCoroutine(DisplayCoroutine());
            Tooltip.CurrentUser = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }

        private IEnumerator DisplayCoroutine()
        {
            yield return new WaitForSeconds(WAIT_SECONDS);

            if (Tooltip.Instance != null)
                Tooltip.Instance.Show(_tooltipText);
        }

        private void HideTooltip()
        {
            if (string.IsNullOrEmpty(_tooltipText) || Tooltip.CurrentUser != this)
                return;

            if (Tooltip.Instance != null)
                Tooltip.Instance.Hide();
            StopCoroutine(_displayCoroutine);
        }
    }
}
/************************************
end TooltipTrigger.cs
*************************************/
