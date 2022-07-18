using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Blockstacker.Common
{
    public static class FileLoading
    {
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

        public static async Task<Texture2D> LoadTextureFromUrl(string path, bool isFile = true)
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
            catch (Exception)
            {
                return null;
            }

            return texture;
                
        }

        public static async Task<AudioClip> LoadAudioClipFromUrl(string path, bool isFile = true)
        {
            if (isFile && GetFileType(path) != FileType.AudioClip) return null;
            
            var extension = Path.GetExtension(path).Remove(0,1);
            var audioType = GetAudioType(extension);

            if (audioType is null) return null;

            var requestUrl = isFile ? $"file://{path}" : path;
            
            using var request = UnityWebRequestMultimedia.GetAudioClip(
                requestUrl,
                (AudioType) audioType
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
            catch (Exception)
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

        private static AudioType? GetAudioType(string extension) => extension switch
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


    public enum FileType : byte
    {
        Texture,
        Video,
        AudioClip,
        Invalid
    }
}