using UnityEngine;

namespace Blockstacker.Settings
{
    public class SettingSaver : MonoBehaviour
    {
        public void Save() => AppSettings.TrySave();
    }
}