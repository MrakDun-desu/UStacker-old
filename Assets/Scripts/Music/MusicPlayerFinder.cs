using UnityEngine;

namespace Blockstacker.Music
{
    public class MusicPlayerFinder : MonoBehaviour
    {
        private MusicPlayer _musicPlayer;

        private void Awake() => FindMusicPlayer();

        private void FindMusicPlayer()
        {
            if (_musicPlayer == null)
                _musicPlayer = FindObjectOfType<MusicPlayer>();
        }

        public void PlayRepeating(string trackName)
        {
            FindMusicPlayer();
            _musicPlayer.PlayImmediate(trackName);
        }

        public void Play(string trackName)
        {
            FindMusicPlayer();
            _musicPlayer.PlayImmediateWithoutLoop(trackName);
        }

        public void PlayGameTrack()
        {
            FindMusicPlayer();
            _musicPlayer.PlayGameTrackImmediate();
        }

        public void PlayVictoryTrack()
        {
            FindMusicPlayer();
            _musicPlayer.PlayVictoryTrackImmediate();
        }

        public void PlayLossTrack()
        {
            FindMusicPlayer();
            _musicPlayer.PlayLossMusicImmediate();
        }

        public void StopPlaying()
        {
            FindMusicPlayer();
            _musicPlayer.StopPlaying();
        }

        public void Quieten()
        {
            FindMusicPlayer();
            _musicPlayer.StartCoroutine(_musicPlayer.MuteSourceOverTime());
        }
    }
}