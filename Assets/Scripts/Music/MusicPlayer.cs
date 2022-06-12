using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Blockstacker.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoSingleton<MusicPlayer>
    {
        [SerializeField] private List<MusicEntry> _menuMusic;
        [SerializeField] private List<MusicEntry> _gameMusic;
        [Range(0, 10)] [SerializeField] private float _switchInterval;
        [Range(0, 10)] [SerializeField] private float _quietenTime = 1f;
        [Range(0, 0.1f)] [SerializeField] private float _quietenInterval = .01f;
        [SerializeField] private SoundPackLoader _soundPackLoader;

        private AudioSource _audioSource;

        private const string MENU_STRING = "Scene_Menu";
        private const string GAME_STRING = "Scene_Game";
        private string _currentSceneType = MENU_STRING;
        private float _nextSongStartTime;
        private float _timeUntilQuiet;

        public void PlayImmediateWithoutLoop(string trackName)
        {
            PlayImmediate(trackName);
            _audioSource.loop = false;
        }
        
        public void PlayImmediate(string trackName)
        {
            _nextSongStartTime = float.PositiveInfinity;
            _timeUntilQuiet = 0;
            _audioSource.Stop();
            _audioSource.clip = _soundPackLoader.Music.Content[trackName];
            _audioSource.volume = 1;
            ResumeNormalPlaying();
        }

        public void PlayGameTrackImmediate()
        {
            var trackName = GetRandomMusicEntry(_gameMusic);
            PlayImmediate(trackName);
        }

        private void ResumeNormalPlaying()
        {
            _audioSource.Play();
            _audioSource.loop = true;
        }
        
        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();

            PickAndPlayNewTrack(_menuMusic);
            
            SceneManager.sceneLoaded += OnSceneChanged;
        }

        private void PickAndPlayNewTrack(IReadOnlyList<MusicEntry> entries)
        {
            _nextSongStartTime = Time.time + _switchInterval;
            var trackName = GetRandomMusicEntry(entries);
            var nextSong = _soundPackLoader.Music.Content[trackName];

            StartCoroutine(PlayNextTrackCor(nextSong));
        }

        private IEnumerator PlayNextTrackCor(AudioClip nextSong)
        {
            yield return new WaitForSeconds(_switchInterval + .1f);
            if (Time.time <= _nextSongStartTime) yield break;
            
            _audioSource.clip = nextSong;
            _audioSource.volume = 1;
            ResumeNormalPlaying();
        }

        private IEnumerator MuteSourceOverTime()
        {
            _timeUntilQuiet = _quietenTime;
            while (_timeUntilQuiet > 0)
            {
                yield return new WaitForSeconds(_quietenInterval);
                _timeUntilQuiet -= _quietenInterval;
                _audioSource.volume = _timeUntilQuiet / _quietenTime;
            }
        }

        private void OnSceneChanged(Scene newScene, LoadSceneMode sceneMode)
        {
            if (sceneMode == LoadSceneMode.Additive) return;
            if (newScene.name.StartsWith(_currentSceneType)) return;

            StartCoroutine(MuteSourceOverTime());
            
            if (newScene.name.StartsWith(MENU_STRING))
            {
                PickAndPlayNewTrack(_menuMusic);
                _currentSceneType = MENU_STRING;
            } else if (newScene.name.StartsWith(GAME_STRING))
            {
                PickAndPlayNewTrack(_gameMusic);
                _currentSceneType = GAME_STRING;
            }
        }

        private static string GetRandomMusicEntry(IReadOnlyList<MusicEntry> entries)
        {
            var effectiveCount = entries.Aggregate(0, (current, entry) => current + (int) entry.TrackFrequency);
            var picked = Random.Range(0, effectiveCount);
            var pickedCache = picked;

            for (var i = 0; i <= pickedCache; i++)
            {
                var currentEntry = entries[i];
                picked -= (int) currentEntry.TrackFrequency;
                if (picked < 0)
                {
                    return currentEntry.TrackName;
                }
            }

            return entries[0].TrackName;
        }
    }
}