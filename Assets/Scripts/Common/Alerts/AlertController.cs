using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Blockstacker.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.Common.Alerts
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
            _closeButton.onClick.AddListener(() => StartCoroutine(RemoveAlertCor()));
            _ = FadeInCor();
            StartCoroutine(WaitForRemoveAlertCor());
        }

        private IEnumerator WaitForRemoveAlertCor()
        {
            yield return new WaitForSeconds(_visibleInterval);
            StartCoroutine(RemoveAlertCor());
        }

        private IEnumerator RemoveAlertCor()
        {
            LeanTween.moveY(_controlledTransform, _controlledTransform.localPosition.y + _movement, _appearTime)
                .setEaseInOutSine().setOnComplete(() => Destroy(gameObject));
            
            var startTime = Time.unscaledTime;
            while (Time.unscaledTime < startTime + _appearTime)
            {
                var currentAlpha = (startTime + _appearTime - Time.unscaledTime) / _appearTime;
                foreach (var image in _controlledImages)
                    image.color = image.color.WithAlpha(currentAlpha);

                foreach (var text in _controlledTexts)
                    text.alpha = currentAlpha;
                
                yield return new WaitForSeconds(1f / 60f);
            }
        }

        private async Task FadeInCor()
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed.TotalSeconds < _appearTime)
            {
                var currentAlpha = (float)stopwatch.Elapsed.TotalSeconds / _appearTime;
                foreach (var image in _controlledImages)
                    image.color = image.color.WithAlpha(currentAlpha);

                foreach (var text in _controlledTexts)
                    text.alpha = currentAlpha;

                await Task.Delay(16);
            }
            foreach (var image in _controlledImages)
                image.color = image.color.WithAlpha(1);

            foreach (var text in _controlledTexts)
                text.alpha = 1;
        }
    }
}