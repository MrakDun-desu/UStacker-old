using UnityEngine;

namespace Blockstacker.Common
{
    public class MonoSingleton : MonoBehaviour
    {
        private static MonoSingleton _instance;

        protected virtual void Awake()
        {
            transform.parent = null;
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            if (_instance != this) {
                Destroy(gameObject);
            }
        }
    }
}