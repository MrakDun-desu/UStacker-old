using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace UStacker.GlobalSettings.Music
{
    [Serializable]
    public class MusicConfiguration
    {
        public List<string> GameMusic = new();
        public List<string> MenuMusic = new();
        public MusicGroupDictionary GameMusicGroups = new();
        [JsonIgnore] private List<string> _defaultMusic = new();

        public void SetDefaultMusic(List<string> value)
        {
            _defaultMusic = value;
        }

        public void Rewrite(MusicConfiguration other)
        {
            UpdateGroup(GameMusic, other.GameMusic, false);
            UpdateGroup(MenuMusic, other.MenuMusic);

            if (other.GameMusicGroups.Count == 0) return;

            GameMusicGroups.Clear();
            foreach (var (key, group) in other.GameMusicGroups)
            {
                for (var i = 0; i < group.Count; i++)
                {
                    if (GameMusic.Contains(group[i])) continue;
                    group.RemoveAt(i);
                    i--;
                }

                if (group.Count > 0)
                    GameMusicGroups.Add(key, group);
            }

        }

        private void UpdateGroup(List<string> mine, IReadOnlyCollection<string> other, bool removeFromGame = true)
        {
            if (other.Count == 0) return;
            mine.Clear();
            var newMusic = other.Where(
                str => SoundPackLoader.Music.ContainsKey(str) || _defaultMusic.Contains(str)
            ).ToArray();
            mine.AddRange(newMusic);
            if (!removeFromGame) return;

            foreach (var clipName in newMusic) GameMusic.Remove(clipName);
        }
    }
}