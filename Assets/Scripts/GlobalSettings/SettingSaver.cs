using UnityEngine;

namespace Blockstacker.GlobalSettings
{
    public class SettingSaver : MonoBehaviour
    {
        public void Save()
        {
            AppSettings.TrySave();
        }
    }
}