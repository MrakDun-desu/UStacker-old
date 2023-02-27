using System.Diagnostics;
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
        [SerializeField] private UnityEvent RestartEvent;
        [SerializeField] private UnityEvent PauseEvent;

        private readonly Stopwatch _stopwatch = new();

        private double _timeScale = 1d;
        private double _offset;

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

        public void SetTime(double value)
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
            
            _sfxPlayer.RepressSfx = true;
            _offset = value;
            TimeScale = oldTimeScale;
            
            _inputProcessor.Update(CurrentTime, true);
            _sfxPlayer.RepressSfx = false;
        }
    }
}