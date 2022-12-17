using System;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameTimer : MonoBehaviour
    {
        private bool _isPaused;
        private bool _isRunning;
        private double _pausedTime;
        private double _pauseStartTime;
        private double _startTime;

        public double EffectiveStartTime
        {
            get
            {
                if (_isRunning)
                    return _startTime + _pausedTime;
                return _startTime + _pausedTime + Time.realtimeSinceStartupAsDouble - _pauseStartTime;
            }
        }

        public double CurrentTime => Math.Max(Time.realtimeSinceStartupAsDouble - EffectiveStartTime, 0d);

        public TimeSpan CurrentTimeAsSpan => TimeSpan.FromSeconds(CurrentTime);

        public event Action<double> TimeSet;
        public event Action BeforeStarted;

        public void StartTiming()
        {
            BeforeStarted?.Invoke();
            _pausedTime = 0d;
            _startTime = Time.realtimeSinceStartupAsDouble;
            _isRunning = true;
        }

        public void ResumeTiming()
        {
            if (_isRunning || !_isPaused) return;
            _pausedTime += Time.realtimeSinceStartupAsDouble - _pauseStartTime;
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
            _pausedTime = 0d;
            _startTime = Time.realtimeSinceStartupAsDouble;
            _pauseStartTime = Time.realtimeSinceStartupAsDouble;
            _isRunning = false;
        }

        public void SetTime(float value)
        {
            var functionStartTime = Time.realtimeSinceStartupAsDouble;
            var newTime = (double) value;
            _startTime = functionStartTime - newTime;
            _pausedTime = 0d;
            if (_isPaused)
                _pauseStartTime = functionStartTime;

            TimeSet?.Invoke(CurrentTime);
        }
    }
}