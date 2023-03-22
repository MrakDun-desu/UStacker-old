using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UStacker.Common;
using UStacker.Common.Alerts;
using UStacker.Common.Extensions;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Stats;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using UStacker.GlobalSettings;

namespace UStacker.Gameplay
{
    [Serializable]
    public record GameReplay
    {
        public string GameType { get; set; }
        public double GameLength { get; set; }
        public DateTime TimeStamp { get; set; }
        public StatContainer Stats { get; set; }
        public GameSettingsSO.SettingsContainer GameSettings { get; set; }
        public List<InputActionMessage> ActionList { get; set; } = new();
        public List<double> PiecePlacementList { get; set; } = new();

        public void Save()
        {
            var mainStat = GameSettings.Objective.MainStat switch
            {
                MainStat.Score => Stats.Score.ToString(),
                MainStat.Time => GameLength.FormatAsTime(),
                MainStat.LinesCleared => Stats.LinesCleared.ToString(),
                MainStat.GarbageLinesCleared => Stats.GarbageLinesCleared.ToString(),
                MainStat.PiecesUsed => Stats.PiecesPlaced.ToString(),
                _ => throw new ArgumentOutOfRangeException()
            };
            var filename = AppSettings.Gameplay.ReplayNamingFormat
                .Replace("{GameType}", GameType)
                .Replace("{MainStat}", mainStat) +
                TimeStamp.ToLocalTime().ToString(CultureInfo.InvariantCulture) + ".bsrep";

            var invalidChars = new List<char>();
            invalidChars.AddRange(Path.GetInvalidPathChars());
            invalidChars.AddRange(Path.GetInvalidFileNameChars());
            invalidChars.AddRange(new[]
            {
                '/',
                '\\'
            });
            const char invalidCharReplacement = '_';

            filename = invalidChars.Aggregate(filename,
                (current, invalidChar) => current.Replace(invalidChar, invalidCharReplacement));

            var savePath = Path.Combine(PersistentPaths.Replays, GameType, filename);
            if (!File.Exists(savePath))
            {
                _ = WriteIntoFileAsync(savePath);
                return;
            }

            for (var i = 0; ; i++)
            {
                var actualSavePath = savePath + $"_{i}";
                if (File.Exists(actualSavePath)) continue;
                _ = WriteIntoFileAsync(actualSavePath);
                return;
            }
        }

        private async Task WriteIntoFileAsync(string savePath)
        {
            var directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory) && directory is not null)
                Directory.CreateDirectory(directory);

            var serializedReplay = JsonConvert.SerializeObject(this, StaticSettings.ReplaySerializerSettings);
            await File.WriteAllBytesAsync(savePath, await FileHandling.ZipAsync(serializedReplay));

            AlertDisplayer.ShowAlert(new Alert(
                "Replay saved!",
                $"Game replay has been saved into a file {savePath}",
                AlertType.Success));
        }

        public static async Task<(bool, GameReplay)> TryLoad(string path)
        {
            (bool, GameReplay) output = (false, null);
            if (!File.Exists(path)) return output;

            var replayJson = await FileHandling.UnzipAsync(await File.ReadAllBytesAsync(path));

            try
            {
                output.Item2 = JsonConvert.DeserializeObject<GameReplay>(replayJson, StaticSettings.ReplaySerializerSettings);
            }
            catch
            {
                // if we get an exception, output item 1 is false so we know that we got an error
            }

            output.Item1 = true;
            return output;
        }
    }
}