using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace UStacker.Common
{
    public class DefaultAppOpener : MonoBehaviour
    {
        public void OpenInPersistentPath(string path)
        {
            var pathToFile = Path.Combine(Application.persistentDataPath, path);
            OpenFile(pathToFile);
        }

        public static void OpenFile(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                Directory.CreateDirectory(path);
            Process.Start(path);
        }

        public void OpenBlockSkinFolder()
        {
            OpenFile(PersistentPaths.Skins);
        }

        public void OpenBackgroundFolder()
        {
            OpenFile(PersistentPaths.BackgroundPacks);
        }

        public void OpenSoundPackFolder()
        {
            OpenFile(PersistentPaths.SoundPacks);
        }
    }
}