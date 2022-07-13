using System;
using System.Collections.Generic;

namespace Blockstacker.GlobalSettings.Music
{
    [Serializable]
    public class MusicConfiguration
    {
        public List<string> GameMusic = new();
        public List<string> MenuMusic = new();
        public List<string> VictoryMusic = new();
        public List<string> LossMusic = new();
        public MusicGroupDictionary GameMusicGroups = new();

        public void Rewrite(MusicConfiguration other)
        {
            // rewrite game music and music groups if they are defined
            if (other.GameMusic.Count != 0)
                GameMusic = other.GameMusic;
            if (other.GameMusicGroups.Count != 0)
                GameMusicGroups = other.GameMusicGroups;

            // remove all game music that is not loaded from configuration
            for (var i = 0; i < GameMusic.Count; i++)
            {
                var entry = GameMusic[i];
                if (SoundPackLoader.Music.ContainsKey(entry)) continue;
                GameMusic.RemoveAt(i);
                i--;
            }

            // remove all music from groups that isn't in the main music collection
            foreach (var group in GameMusicGroups)
            {
                for (var i = 0; i < group.Value.Count; i++)
                {
                    if (GameMusic.Contains(group.Value[i])) continue;
                    group.Value.RemoveAt(i);
                    i--;
                }
            }

            // add only the clips that are in the game music to predefined groups
            for (var i = 0; i < other.GameMusic.Count; i++)
            {
                if (other.MenuMusic.Contains(other.GameMusic[i]))
                {
                    MenuMusic.Add(other.GameMusic[i]);
                    GameMusic.Remove(other.GameMusic[i]);
                }
                if (other.VictoryMusic.Contains(other.GameMusic[i]))
                {
                    VictoryMusic.Add(other.GameMusic[i]);
                    GameMusic.Remove(other.GameMusic[i]);
                }
                if (other.LossMusic.Contains(other.GameMusic[i]))
                {
                    LossMusic.Add(other.GameMusic[i]);
                    GameMusic.Remove(other.GameMusic[i]);
                }
            }
        }
    }
}