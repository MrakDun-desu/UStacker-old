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
            if (!Directory.Exists(pathToFile) && !File.Exists(pathToFile))
                Directory.CreateDirectory(pathToFile);
            Process.Start(pathToFile);
        }
    }
}