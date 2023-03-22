using System;
using System.Collections;
using UStacker.Common.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace UStacker.Common.Alerts
{
    public class AlertController : MonoBehaviour
    {
        [SerializeField] private float _visibleInterval = 5f;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Sprite _infoSprite;
        [SerializeField] private Sprite _successSprite;
        [SerializeField] private Sprite _warningSprite;
        [SerializeField] private Sprite _errorSprite;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private RectTransform _controlledTransform;
        [SerializeField] private float _movement = 200f;
        [SerializeField] private float _appearTime = .5f;

        private bool _removeStarted;
        private bool _activeInPool;
        private Image[] _controlledImages = Array.Empty<Image>();
        private TMP_Text[] _controlledTexts = Array.Empty<TMP_Text>();
        
        public ObjectPool<AlertController> SourcePool { get; set; }

        private void Awake()
        {
            _controlledImages = GetComponentsInChildren<Image>();
            _controlledTexts = GetComponentsInChildren<TMP_Text>();
        }

        public void Initialize(Alert alert)
        {
            _removeStarted = false;
            _activeInPool = true;
            _backgroundImage.sprite = alert.AlertType switch
            {
                AlertType.Success => _successSprite,
                AlertType.Info => _infoSprite,
                AlertType.Warning => _warningSprite,
                AlertType.Error => _errorSprite,
                _ => throw new ArgumentOutOfRangeException()
            };

            _title.text = alert.Title;
            _text.text = alert.Text;
            
            _text.ForceMeshUpdate();
            var newTextSize = new Vector2(_text.rectTransform.sizeDelta.x, _text.preferredHeight);
            var newContainerSize = newTextSize.y != 0 ? newTextSize : _title.rectTransform.sizeDelta;

            _controlledTransform.localPosition = new Vector3();
            _text.rectTransform.sizeDelta = newTextSize;
            _controlledTransform.sizeDelta = newContainerSize;
            ((RectTransform) transform).sizeDelta = newContainerSize;
            
            _closeButton.onClick.AddListener(RemoveAlert);
            SetAlpha(0);
            DOTween.To(GetAlpha, SetAlpha, 1, _appearTime).SetEase(Ease.Linear);
            StartCoroutine(WaitForRemoveAlertCor());
        }

        private IEnumerator WaitForRemoveAlertCor()
        {
            yield return new WaitForSeconds(_visibleInterval);
            if (!_removeStarted)
                RemoveAlert();
        }

        private void RemoveAlert()
        {
            _removeStarted = true;
            _controlledTransform.DOMoveY(_movement, _appearTime).SetRelative(true)
                .OnComplete(() =>
                {
                    if (!_activeInPool) return;
                    SourcePool.Release(this);
                    _activeInPool = false;
                });
            DOTween.To(GetAlpha, SetAlpha, 0, _appearTime).SetEase(Ease.Linear);
        }

        private void SetAlpha(float value)
        {
            foreach (var image in _controlledImages)
                image.color = image.color.WithAlpha(value);
            foreach (var text in _controlledTexts)
                text.alpha = value;
        }

        private float GetAlpha()
        {
            return _controlledImages[0].color.a;
        }
    }
}