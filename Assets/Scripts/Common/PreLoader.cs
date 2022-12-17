using UnityEngine;

namespace Blockstacker.Common
{
    public class PreLoader : MonoBehaviour
    {
        private bool _loaded;

        private void LateUpdate()
        {
            if (_loaded) return;
            gameObject.SetActive(false);
            _loaded = true;
        }
    }
}