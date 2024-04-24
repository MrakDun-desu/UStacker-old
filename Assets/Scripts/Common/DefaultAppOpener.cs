
/************************************
DefaultAppOpener.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace UStacker.Common
{
    public class DefaultAppOpener : MonoBehaviour
    {
        public static void OpenFile(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                Directory.CreateDirectory(path);
            Process.Start(path);
        }
    }
}
/************************************
end DefaultAppOpener.cs
*************************************/
