
/************************************
PreLoader.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;

namespace UStacker.Common
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
/************************************
end PreLoader.cs
*************************************/
