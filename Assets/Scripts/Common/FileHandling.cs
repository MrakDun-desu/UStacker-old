
/************************************
FileHandling.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UStacker.Common
{
    public static class FileHandling
    {
        public static async Task<Texture2D> LoadTextureFromUrlAsync(string path, bool isFile = true)
        {
            if (isFile && GetFileType(path) != FileType.Texture) return null;

            var requestUrl = isFile ? $"file://{path}" : path;

            using var request = UnityWebRequestTexture.GetTexture(requestUrl);
            request.SendWebRequest();

            Texture2D texture;
            try
            {
                while (!request.isDone) await Task.Delay(10);

                if (request.result != UnityWebRequest.Result.Success)
                    return null;
                texture = DownloadHandlerTexture.GetContent(request);
            }
            catch
            {
                return null;
            }

            return texture;
        }

        public static async Task<AudioClip> LoadAudioClipFromUrlAsync(string path, bool isFile = true)
        {
            if (isFile && GetFileType(path) != FileType.AudioClip) return null;

            var extension = Path.GetExtension(path).Remove(0, 1);
            var audioType = GetAudioType(extension);

            if (audioType is not { } usedType) return null;

            var requestUrl = isFile ? $"file://{path}" : path;

            using var request = UnityWebRequestMultimedia.GetAudioClip(
                requestUrl,
                usedType
            );

            request.SendWebRequest();

            AudioClip clip;
            try
            {
                while (!request.isDone) await Task.Delay(10);

                if (request.result != UnityWebRequest.Result.Success)
                    return null;

                clip = DownloadHandlerAudioClip.GetContent(request);
            }
            catch
            {
                return null;
            }

            return clip;
        }

        public static FileType GetFileType(string path)
        {
            if (!File.Exists(path)) return FileType.Invalid;
            var extension = Path.GetExtension(path).Remove(0, 1);

            if (ValidTextureExtensions.Contains(extension)) return FileType.Texture;
            if (ValidAudioExtensions.Contains(extension)) return FileType.AudioClip;
            return ValidVideoExtensions.Contains(extension) ? FileType.Video : FileType.Invalid;
        }

        private static AudioType? GetAudioType(string extension)
        {
            return extension switch
            {
                "mp3" => AudioType.MPEG,
                "ogg" => AudioType.OGGVORBIS,
                "wav" => AudioType.WAV,
                "aiff" or "aif" => AudioType.AIFF,
                "mod" => AudioType.MOD,
                "it" => AudioType.IT,
                "s3m" => AudioType.S3M,
                "xm" => AudioType.XM,
                _ => null
            };
        }

        public static async Task<byte[]> ZipAsync(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using var sourceStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            await using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                await sourceStream.CopyToAsync(gZipStream);
            }

            return outputStream.ToArray();
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using var sourceStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                sourceStream.CopyTo(gZipStream);
            }

            return outputStream.ToArray();
        }

        public static async Task<string> UnzipAsync(byte[] bytes)
        {
            using var sourceStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            await using (var gZipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                await gZipStream.CopyToAsync(outputStream);
            }

            return Encoding.UTF8.GetString(outputStream.ToArray());
        }

        public static string Unzip(byte[] bytes)
        {
            using var sourceStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using (var gZipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                gZipStream.CopyTo(outputStream);
            }

            return Encoding.UTF8.GetString(outputStream.ToArray());
        }

        public static async Task CopyDirectoryRecursivelyAsync(string sourcePath, string destinationPath)
        {
            var dir = new DirectoryInfo(sourcePath);
            if (!Directory.Exists(destinationPath) || !dir.Exists)
                return;

            var taskList = new List<Task>();
            // not converting to LINQ for better readability
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var file in dir.EnumerateFiles())
            {
                var targetFilePath = Path.Combine(destinationPath, file.Name);
                taskList.Add(CopyFileAsync(file.FullName, targetFilePath));
            }

            foreach (var subDir in dir.EnumerateDirectories())
            {
                var newDestinationPath = Path.Combine(destinationPath, subDir.Name);
                Directory.CreateDirectory(newDestinationPath);

                taskList.Add(CopyDirectoryRecursivelyAsync(subDir.FullName, newDestinationPath));
            }

            await Task.WhenAll(taskList);
        }

        public static async Task CopyFileAsync(string source, string destination)
        {
            if (!File.Exists(source)) return;

            await using var sourceReader = File.OpenRead(source);
            await using var destinationWriter = File.Create(destination);
            await sourceReader.CopyToAsync(destinationWriter);
        }

        public static void CreateDirectoriesRecursively(string path)
        {
            if (Directory.Exists(path))
                return;

            CreateDirectoriesRecursively(Path.GetDirectoryName(path));
            Directory.CreateDirectory(path);
        }

        #region Valid extension collections

        private static readonly string[] ValidTextureExtensions =
        {
            "jpg",
            "jpeg",
            "png"
        };

        private static readonly string[] ValidVideoExtensions =
        {
            "avi",
            "dv",
            "m4v",
            "mov",
            "mp4", // definitely works
            "mpg",
            "mpeg",
            "ogv",
            "vp8",
            "wmv"
        };

        private static readonly string[] ValidAudioExtensions =
        {
            "mp3",
            "ogg",
            "wav",
            "aiff",
            "aif",
            "mod",
            "it",
            "s3m",
            "xm"
        };

        #endregion
    }


    public enum FileType : byte
    {
        Texture,
        Video,
        AudioClip,
        Invalid
    }
}
/************************************
end FileHandling.cs
*************************************/
