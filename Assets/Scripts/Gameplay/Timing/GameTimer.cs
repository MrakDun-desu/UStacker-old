using System.Diagnostics;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.InputProcessing;
using UStacker.Gameplay.SoundEffects;

namespace UStacker.Gameplay.Timing
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private SoundEffectsPlayer _sfxPlayer;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private float _maximumTweenDuration = 0.5f;
        [SerializeField] private UnityEvent RestartEvent;
        [SerializeField] private UnityEvent PauseEvent;

        private readonly Stopwatch _stopwatch = new();

        private double _timeScale = 1d;
        private double _offset;
        private TweenerCore<double, double, NoOptions> _timeTween;

        public bool IsRunning => _stopwatch.IsRunning;

        public double TimeScale
        {
            get => _timeScale;
            set
            {
                var newTime = CurrentTime / value;
                _timeScale = value;
                _offset = newTime;

                if (_stopwatch.IsRunning)
                    _stopwatch.Restart();
                else
                    _stopwatch.Reset();
            }
        }

        public double CurrentTime => (_offset + _stopwatch.Elapsed.TotalSeconds) * TimeScale;


        public void StartTiming()
        {
            _stopwatch.Start();
        }

        public void PauseTiming()
        {
            _stopwatch.Stop();
        }

        public void ResetTiming()
        {
            TimeScale = 1d;
            _offset = 0d;
            _stopwatch.Reset();
        }

        public void SetTime(double value, bool repressSfx = true)
        {
            var oldTimeScale = TimeScale;
            var wasRunning = _stopwatch.IsRunning;
            var restarted = false;
            if (value < CurrentTime)
            {
                restarted = true;
                RestartEvent.Invoke();
            }
            else
                TimeScale = 1;

            if (wasRunning)
                _stopwatch.Restart();
            else
            {
                if (restarted)
                    PauseEvent.Invoke();

                _stopwatch.Reset();
            }

            _offset = value;
            TimeScale = oldTimeScale;

            _sfxPlayer.RepressSfx = repressSfx;
            _inputProcessor.Update(CurrentTime, true);
            _sfxPlayer.RepressSfx = false;
        }

        public void TweenTimeForward(double targetTime)
        {
            var currentTime = CurrentTime;
            var tweenDuration = (float) (targetTime - currentTime);
            if (tweenDuration < 0)
                return;

            // if (tweenDuration > _maximumTweenDuration)
            // {
            //     SetTime(Math.Max(targetTime - _maximumTweenDuration, 0d));
            //     currentTime = CurrentTime;
            //     tweenDuration = (float) (targetTime - currentTime);
            // }

            _timeTween?.Kill();
            _timeTween = DOTween
                .To(TweenGetter, TweenSetter, targetTime, Mathf.Min(tweenDuration, _maximumTweenDuration))
                .OnComplete(NullTween).SetEase(Ease.Linear);

            double TweenGetter() => CurrentTime;
            void TweenSetter(double value) => SetTime(value, false);
            void NullTween() => _timeTween = null;
        }
    }
}