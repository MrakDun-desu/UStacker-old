using UStacker.Common.Alerts;
using UnityEngine;
#if !UNITY_EDITOR
using System;
using System.IO;
#endif

namespace UStacker.Common
{
    public static class Logger
    {
        public static void Log(object obj)
        {
#if UNITY_EDITOR
            Debug.Log(obj);
#else
            File.AppendAllText(PersistentPaths.DebugLogs, $"[{DateTime.Now}] {obj}\n");
#endif
        }

        public static void LogAlert(Alert alert)
        {
            Log($"{alert.AlertType.ToString().ToUpper()}: {alert.Title}\n\t{alert.Text}");
        }
    }
}