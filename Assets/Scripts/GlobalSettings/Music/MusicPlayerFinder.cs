using Blockstacker.Common;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Music
{
    public class MusicPlayerFinder : MonoBehaviour
    {
        public string GameType { get; set; }

        public void PlayGameTrackByType()
        {
            MusicPlayer.Instance.PlayTrackByGameTypeImmediate(GameType);
        }

        public void PlayVictoryTrack()
        {
            MusicPlayer.Instance.PlayVictoryTrack();
        }

        public void PlayLossTrack()
        {
            MusicPlayer.Instance.PlayLossTrack();
        }

        public void StopPlaying()
        {
            MusicPlayer.Instance.StopPlaying();
        }

        public void Quieten()
        {
            MusicPlayer.Instance.StartCoroutine(MusicPlayer.Instance.MuteSourceOverTime());
        }
    }
}