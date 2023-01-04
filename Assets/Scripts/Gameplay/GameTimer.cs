using System;
using UnityEngine;
using UStacker.Common.Extensions;
using UStacker.Gameplay.SoundEffects;

namespace UStacker.Gameplay
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private SoundEffectsPlayer _sfxPlayer;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private GameStateManager _stateManager;

        private bool _isPaused;
        private bool _isRunning;
        private double _pauseStartTime;
        private double _startTime;
        private double _timeScale = 1d;

        public double EffectiveStartTime
        {
            get
            {
                if (_isRunning)
                    return _startTime;
                return _startTime + Time.realtimeSinceStartupAsDouble - _pauseStartTime;
            }
        }

        public double TimeScale
        {
            get => _timeScale;
            set
            {
                var newRealtime = Time.realtimeSinceStartupAsDouble;
                var newTime = CurrentTime / value;
                _timeScale = value;
                _startTime = newRealtime - newTime;

                if (_isPaused)
                    _pauseStartTime = newRealtime;
            }
        }

        public double CurrentTime => Math.Max((Time.realtimeSinceStartupAsDouble - EffectiveStartTime) * TimeScale, 0d);

        public TimeSpan CurrentTimeAsSpan => TimeSpan.FromSeconds(CurrentTime);

        public event Action BeforeStarted;

        public void StartTiming()
        {
            BeforeStarted?.Invoke();
            _startTime = Time.realtimeSinceStartupAsDouble;
            _isRunning = true;
        }

        public void ResumeTiming()
        {
            if (_isRunning || !_isPaused) return;
            _startTime += Time.realtimeSinceStartupAsDouble - _pauseStartTime;
            _isRunning = true;
            _isPaused = false;
        }

        public void PauseTiming()
        {
            if (!_isRunning) return;

            _pauseStartTime = Time.realtimeSinceStartupAsDouble;
            _isRunning = false;
            _isPaused = true;
        }

        public void ResetTiming()
        {
            TimeScale = 1d;
            _startTime = Time.realtimeSinceStartupAsDouble;
            _pauseStartTime = Time.realtimeSinceStartupAsDouble;
            _isRunning = false;
        }

        public void SetTime(double value)
        {
            Debug.Log($"Time has been set to {value.FormatAsTime()}");
            _sfxPlayer.RepressSfx = true;
            var oldTime = CurrentTime;
            var newRealtime = Time.realtimeSinceStartupAsDouble;
            _startTime = newRealtime - (value / TimeScale);

            if (_isPaused)
                _pauseStartTime = newRealtime;

            if (CurrentTime < oldTime)
                _stateManager.Restart();

            _inputProcessor.Update(value, true);
            _sfxPlayer.RepressSfx = false;
        }
    }
}