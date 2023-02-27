using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UStacker.Common.UI
{
    public class Tooltip : MonoSingleton<Tooltip>
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _background;

        public static Tooltip Instance => _instance;
        public static GameObject CurrentUser { get; set; }

        private void Start()
        {
            Hide();
        }

        private void LateUpdate()
        {
            _background.transform.position = Mouse.current.position.ReadValue();
        }

        public void Show(string tooltipText)
        {
            gameObject.SetActive(true);

            _text.text = tooltipText;
            var textSize = new Vector2(_text.preferredWidth, _text.preferredHeight);
            _text.rectTransform.sizeDelta = textSize;
            _text.ForceMeshUpdate();
            textSize = new Vector2(_text.preferredWidth, _text.preferredHeight);
            _text.rectTransform.sizeDelta = textSize;
            _background.sizeDelta = textSize;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}