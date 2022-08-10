using System;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameTimer : MonoBehaviour
    {
        private bool _isRunning;
        private double _startTime;
        private bool _isPaused;
        private double _pauseStartTime;
        private double _pausedTime;
        
        public double EffectiveStartTime
        {
            get
            {
                if (_isRunning)
                    return _startTime + _pausedTime;
                return _startTime + _pausedTime + Time.realtimeSinceStartupAsDouble - _pauseStartTime;
            }
        }

        public double CurrentTime => Time.realtimeSinceStartupAsDouble - EffectiveStartTime;

        public TimeSpan CurrentTimeAsSpan => TimeSpan.FromSeconds(CurrentTime);

        public void StartTiming()
        {
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
    }
}