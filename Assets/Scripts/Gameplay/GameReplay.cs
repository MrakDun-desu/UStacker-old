using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Stats;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;
using Newtonsoft.Json;

namespace Blockstacker.Gameplay
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

        public void Save(string gameType)
        {
            var filename = AppSettings.Gameplay.ReplayNamingFormat
                .Replace("{GameType}", gameType)
                .Replace("{Timestamp}", TimeStamp.ToLocalTime().ToString(CultureInfo.InvariantCulture));

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
            filename += ".bsrep";

            var savePath = Path.Combine(PersistentPaths.Replays, gameType, filename);
            if (!File.Exists(savePath))
            {
                _ = WriteIntoFileAsync(savePath);
                return;
            }

            for (var i = 0;; i++)
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
            await File.WriteAllBytesAsync(savePath, FileLoading.Zip(serializedReplay));

            _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                "Replay saved!",
                $"Game replay has been saved into a file {savePath}",
                AlertType.Success));
        }

        public static bool TryLoad(string path, out GameReplay output)
        {
            output = default;
            if (!File.Exists(path)) return false;

            var replayJson = FileLoading.Unzip(File.ReadAllBytes(path));

            try
            {
                output = JsonConvert.DeserializeObject<GameReplay>(replayJson, StaticSettings.ReplaySerializerSettings);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}