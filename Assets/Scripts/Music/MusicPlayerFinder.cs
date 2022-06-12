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

        public void PlayRandomGameTrack()
        {
            FindMusicPlayer();
            _musicPlayer.PlayGameTrackImmediate();
        }
    }
}