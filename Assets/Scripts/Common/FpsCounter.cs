using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Blockstacker.Common
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private float _interval = 1f;
        private TMP_Text _text;
        private Stopwatch _stopwatch;
        private double _offset;

        private DateTime _timeStarted;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _stopwatch = new Stopwatch();
            _timeStarted = DateTime.Now;
            _stopwatch.Start();

            StartCoroutine(ShowFpsCor());
        }

        private IEnumerator ShowFpsCor()
        {
            _offset = Time.realtimeSinceStartupAsDouble - _stopwatch.Elapsed.TotalSeconds;
            while (true)
            {
                var timeFromLoad = Time.realtimeSinceStartupAsDouble;
                var timeFromStopwatch = _stopwatch.Elapsed.TotalSeconds;
                var loadTimeText = Math.Round(timeFromLoad, 2).ToString(CultureInfo.InvariantCulture);
                var stopwatchTimeText = Math.Round(timeFromStopwatch, 2).ToString(CultureInfo.InvariantCulture);
                _text.text =
                    $"Load time: {loadTimeText}\nStopwatch time: {stopwatchTimeText}\nStarting offset: {Math.Round(_offset, 5)}\nCurrent offset: {Math.Round(timeFromLoad - timeFromStopwatch, 5)}" +
                    $"\nTime started: {_timeStarted}";

                yield return new WaitForSeconds(_interval);
            }
        }
    }
}