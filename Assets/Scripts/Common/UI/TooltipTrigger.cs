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
            
            Tooltip.Instance.Show(_tooltipText);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(_tooltipText))
                return;
            
            _displayCoroutine = StartCoroutine(DisplayCoroutine());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(_tooltipText))
                return;
            
            Tooltip.Instance.Hide();
            StopCoroutine(_displayCoroutine);
        }
    }
}