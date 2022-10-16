using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Blockstacker.Common
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
    }
}