using UnityEngine;

namespace UStacker.GlobalSettings.Music
{
    public class MusicPlayerFinder : MonoBehaviour
    {
        public string GameType { get; set; }

        public void PlayGameTrackByType()
        {
            MusicPlayer.Instance.PlayTrackByGameTypeImmediate(GameType);
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