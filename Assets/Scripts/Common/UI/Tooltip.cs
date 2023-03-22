using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UStacker.Common.UI
{
    public class Tooltip : MonoSingleton<Tooltip>
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _background;
        [Range(0.2f, 0.8f)] [SerializeField] private float _maxWidthScreenPercent = 0.4f;
        
        public static Tooltip Instance => _instance;
        public static MonoBehaviour CurrentUser { get; set; }

        private Camera _camera;

        private void Start()
        {
            Hide();
        }

        private void LateUpdate()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var parentWidth = ((RectTransform) transform).sizeDelta.x;
            var backgroundWidth = _background.sizeDelta.x;
            
            _background.position = mousePosition;
            var newPosition = _background.anchoredPosition;
            
            if (backgroundWidth + newPosition.x + 10 > parentWidth)
                newPosition.x = parentWidth - backgroundWidth - 10;

            _background.anchoredPosition = newPosition;

        }

        public void Show(string tooltipText)
        {
            tooltipText = tooltipText.Replace("\r", " ");
            tooltipText = tooltipText.Replace("\n", " ");
            while (tooltipText.Contains("  "))
                tooltipText = tooltipText.Replace("  ", " ");
            
            gameObject.SetActive(true);

            var maxWidth = ((RectTransform)transform).sizeDelta.x * _maxWidthScreenPercent;
            
            _text.text = tooltipText;
            var textSize = new Vector2(Mathf.Min(_text.preferredWidth, maxWidth), _text.preferredHeight);
            _text.rectTransform.sizeDelta = textSize;
            _text.ForceMeshUpdate();
            textSize = new Vector2(Mathf.Min(_text.preferredWidth, maxWidth), _text.preferredHeight);
            _text.rectTransform.sizeDelta = textSize;
            _background.sizeDelta = textSize;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}