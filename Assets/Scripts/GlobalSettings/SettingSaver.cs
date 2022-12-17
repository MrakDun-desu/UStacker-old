using UnityEngine;

namespace UStacker.GlobalSettings
{
    public class SettingSaver : MonoBehaviour
    {
        public void Save()
        {
            AppSettings.TrySave();
        }
    }
}