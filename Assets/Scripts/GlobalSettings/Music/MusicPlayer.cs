
/************************************
MusicPlayer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.GlobalSettings.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoSingleton<MusicPlayer>
    {
        private const string MENU_STRING = "Scene_Menu";
        private const string GAME_STRING = "Scene_Game";

        [FormerlySerializedAs("_musicConfiguration")]
        [ContextMenuItem("Copy JSON to clipboard", nameof(CopyToClipboard))]
        public MusicConfiguration Configuration;

        [Range(0, 10)] [SerializeField] private float _switchInterval;
        [Range(0, 10)] [SerializeField] private float _quietenTime = 1f;
        [Range(0, 0.1f)] [SerializeField] private float _quietenInterval = .01f;
        [SerializeField] private AudioClipCollection _defaultMusic = new();

        private AudioSource _audioSource;
        private string _currentSceneType = MENU_STRING;
        private float _nextSongStartTime;
        private float _timeUntilQuiet;

        public static MusicPlayer Instance => _instance;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            Configuration.SetDefaultMusic(_defaultMusic.Select(entry => entry.Key).ToList());
        }

        public void Start()
        {
            SceneManager.sceneLoaded += OnSceneChanged;
            if (Configuration.MenuMusic.TryGetRandomElement(out var nextTrack))
                PlayNextTrack(nextTrack);

            SoundPackLoader.SoundPackChanged += () => PlayTrackByScene(_currentSceneType);
        }

        private void CopyToClipboard()
        {
            var output = JsonConvert.SerializeObject(Configuration, StaticSettings.DefaultSerializerSettings);
            GUIUtility.systemCopyBuffer = output;
        }

        public List<MusicOption> ListAvailableOptions()
        {
            var outList =
                Configuration.GameMusicGroups.Keys.Select(groupName => new MusicOption(OptionType.Group, groupName))
                    .ToList();
            outList.AddRange(Configuration.GameMusic.Select(trackName => new MusicOption(OptionType.Track, trackName)));

            return outList;
        }

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
            if (SoundPackLoader.Music.TryGetValue(trackName, out var usedClip))
                _audioSource.clip = usedClip;
            else if (_defaultMusic.TryGetValue(trackName, out usedClip))
                _audioSource.clip = usedClip;
            else return;
            _audioSource.volume = 1;
            ResumeNormalPlaying();
        }

        public void PlayTrackByGameTypeImmediate(string gameType)
        {
            if (!AppSettings.Sound.GameMusicDictionary.TryGetValue(gameType, out var gameMusicOption))
            {
                if (Configuration.GameMusic.TryGetRandomElement(out var trackName))
                    PlayImmediate(trackName);

                return;
            }

            if (gameMusicOption is null || string.IsNullOrEmpty(gameMusicOption.Name))
            {
                if (Configuration.GameMusic.TryGetRandomElement(out var trackName))
                    PlayImmediate(trackName);

                return;
            }

            switch (gameMusicOption.OptionType)
            {
                case OptionType.Track:
                    PlayImmediate(gameMusicOption.Name);
                    break;
                case OptionType.Group:
                    PlayFromGroupImmediate(gameMusicOption.Name);
                    break;
                case OptionType.Random:
                    if (Configuration.GameMusic.TryGetRandomElement(out var trackName))
                        PlayImmediate(trackName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void PlayFromGroupImmediate(string groupName)
        {
            if (Configuration.GameMusicGroups[groupName].TryGetRandomElement(out var trackName))
                PlayImmediate(trackName);
        }

        public void StopPlaying()
        {
            _audioSource.Stop();
            _nextSongStartTime = float.PositiveInfinity;
        }

        private void ResumeNormalPlaying()
        {
            _audioSource.Play();
            _audioSource.loop = true;
        }

        private void PlayNextTrack(string trackName)
        {
            _nextSongStartTime = Time.time + _switchInterval;

            var songFound = SoundPackLoader.Music.TryGetValue(trackName, out var nextSong);
            if (!songFound)
                songFound = _defaultMusic.TryGetValue(trackName, out nextSong);

            if (songFound)
                StartCoroutine(PlayNextTrackCor(nextSong));
        }

        private IEnumerator PlayNextTrackCor(AudioClip nextTrack)
        {
            yield return new WaitForSeconds(_switchInterval + .01f);
            if (Time.time <= _nextSongStartTime) yield break;

            _timeUntilQuiet = 0;
            _audioSource.clip = nextTrack;
            _audioSource.volume = 1;
            ResumeNormalPlaying();
        }

        public IEnumerator MuteSourceOverTime()
        {
            _timeUntilQuiet = _quietenTime;
            while (_timeUntilQuiet > 0)
            {
                _timeUntilQuiet -= _quietenInterval;
                _audioSource.volume = _timeUntilQuiet / _quietenTime;
                yield return new WaitForSeconds(_quietenInterval);
            }
        }

        private void OnSceneChanged(Scene newScene, LoadSceneMode sceneMode)
        {
            if (sceneMode == LoadSceneMode.Additive) return;
            if (newScene.name.StartsWith(_currentSceneType)) return;

            StartCoroutine(MuteSourceOverTime());

            PlayTrackByScene(newScene.name);
        }

        private void PlayTrackByScene(string sceneName)
        {
            if (sceneName.StartsWith(MENU_STRING))
            {
                if (Configuration.MenuMusic.TryGetRandomElement(out var nextTrack))
                    PlayNextTrack(nextTrack);
                _currentSceneType = MENU_STRING;
            }
            else if (sceneName.StartsWith(GAME_STRING))
            {
                // game scenes handle music playing by themselves
                _currentSceneType = GAME_STRING;
            }
        }
    }
}
/************************************
end MusicPlayer.cs
*************************************/
