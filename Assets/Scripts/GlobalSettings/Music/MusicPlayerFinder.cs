using UnityEngine;

namespace Blockstacker.GlobalSettings.Music
{
    public class MusicPlayerFinder : MonoBehaviour
    {
        private MusicPlayer _musicPlayer;

        private void FindMusicPlayer()
        {
            if (_musicPlayer == null)
                _musicPlayer = FindObjectOfType<MusicPlayer>();
        }

        public void PlayGameTrackByType(string gameType)
        {
            FindMusicPlayer();
            _musicPlayer.PlayTrackByGameTypeImmediate(gameType);
        }

        public void PlayVictoryTrack()
        {
            FindMusicPlayer();
            _musicPlayer.PlayVictoryTrack();
        }

        public void PlayLossTrack()
        {
            FindMusicPlayer();
            _musicPlayer.PlayLossTrack();
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