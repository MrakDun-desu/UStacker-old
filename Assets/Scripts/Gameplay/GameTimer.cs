using System;
using System.Diagnostics;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameTimer : MonoBehaviour
    {
        private readonly Stopwatch _stopwatch = new();
        private TimeSpan _startOffset = TimeSpan.Zero;

        private double _scale = 1;

        public double Scale
        {
            get => _scale;
            set
            {
                if (value == 0)
                {
                    _stopwatch.Stop();
                    return;
                }
                _startOffset = _stopwatch.Elapsed * _scale + _startOffset;
                if (_stopwatch.IsRunning)
                    _stopwatch.Restart();
                else
                    _stopwatch.Reset();
                
                _scale = value;
            }
        }

        public double CurrentTime
        {
            get => (_stopwatch.Elapsed * _scale + _startOffset).TotalSeconds;
            set
            {
                _startOffset = TimeSpan.FromSeconds(value);
                if (_stopwatch.IsRunning)
                    _stopwatch.Restart();
                else
                    _stopwatch.Reset();
            }
        }

        public TimeSpan CurrentTimeAsSpan => _stopwatch.Elapsed * _scale + _startOffset;
        
        public void StartTiming()
        {
            _stopwatch.Start();
        }

        public void StopTiming()
        {
            _stopwatch.Stop();
        }

        public void ResetTiming()
        {
            _stopwatch.Reset();
        }

    }
}