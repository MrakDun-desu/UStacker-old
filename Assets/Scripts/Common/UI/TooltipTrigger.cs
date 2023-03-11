using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UStacker.Common.UI
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea(5, 30)][SerializeField] private string _tooltipText = string.Empty;
        private const float WAIT_SECONDS = 0.5f;

        private Coroutine _displayCoroutine;

        private IEnumerator DisplayCoroutine()
        {
            yield return new WaitForSeconds(WAIT_SECONDS);
            
            if (Tooltip.Instance != null)
                Tooltip.Instance.Show(_tooltipText);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(_tooltipText))
                return;
            
            _displayCoroutine = StartCoroutine(DisplayCoroutine());
            Tooltip.CurrentUser = gameObject;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }

        private void HideTooltip()
        {
            if (string.IsNullOrEmpty(_tooltipText) || Tooltip.CurrentUser != gameObject)
                return;
            
            if (Tooltip.Instance != null)
                Tooltip.Instance.Hide();
            StopCoroutine(_displayCoroutine);
        }

        private void OnDisable()
        {
            HideTooltip();
        }
    }
}