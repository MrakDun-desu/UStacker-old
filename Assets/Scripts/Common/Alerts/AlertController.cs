using System;
using System.Collections;
using UStacker.Common.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
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

        private bool _destroyedFlag;
        private Image[] _controlledImages = Array.Empty<Image>();
        private TMP_Text[] _controlledTexts = Array.Empty<TMP_Text>();

        public void Initialize(Alert alert)
        {
            _controlledImages = GetComponentsInChildren<Image>();
            _controlledTexts = GetComponentsInChildren<TMP_Text>();
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
            _closeButton.onClick.AddListener(RemoveAlert);
            SetAlpha(0);
            DOTween.To(GetAlpha, SetAlpha, 1, _appearTime).SetEase(Ease.Linear);
            StartCoroutine(WaitForRemoveAlertCor());
        }

        private IEnumerator WaitForRemoveAlertCor()
        {
            yield return new WaitForSeconds(_visibleInterval);
            if (!_destroyedFlag)
                RemoveAlert();
        }

        private void RemoveAlert()
        {
            _controlledTransform.DOMoveY(_movement, _appearTime).SetRelative(true)
                .OnComplete(() =>
                {
                    _destroyedFlag = true;
                    Destroy(gameObject);
                });
            DOTween.To(GetAlpha, SetAlpha, 0, _appearTime).SetEase(Ease.Linear);
        }

        private void SetAlpha(float value)
        {
            if (_destroyedFlag)
                return;
            
            foreach (var image in _controlledImages)
                image.color = image.color.WithAlpha(value);
            foreach (var text in _controlledTexts)
                text.alpha = value;
        }

        private float GetAlpha()
        {
            if (_destroyedFlag)
                return 0;
            
            return _controlledImages[0].color.a;
        }
    }
}