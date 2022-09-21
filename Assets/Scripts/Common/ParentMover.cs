using UnityEngine;
using UnityEngine.EventSystems;

namespace Blockstacker.Common
{
    public class ParentMover : MonoBehaviour, IPointerClickHandler
    {
        private RectTransform _parentTransform;
        private Vector3 _dragStartParentPosition;

        private void Start()
        {
            _parentTransform = GetComponentInParent<RectTransform>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Pointer clicked");
        }

        public void OnPointerClick()
        {
            Debug.Log("Pointer clicked");
        }
    }
}