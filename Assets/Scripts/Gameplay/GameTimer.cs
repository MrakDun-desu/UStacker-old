using System;
using System.Diagnostics;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameTimer : MonoBehaviour
    {
        private bool _isRunning;
        private double _startTime;
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
            _startTime = Time.realtimeSinceStartupAsDouble;
            _isRunning = true;
        }

        public void ResumeTiming()
        {
            if (_isRunning) return;
            _pausedTime += Time.realtimeSinceStartupAsDouble - _pauseStartTime;
            _isRunning = true;
        }

        public void StopTiming()
        {
            _pauseStartTime = Time.realtimeSinceStartupAsDouble;
            _isRunning = false;
        }

        public void ResetTiming()
        {
            _pausedTime = 0d;
            _startTime = Time.realtimeSinceStartupAsDouble;
            StopTiming();
        }
    }
}