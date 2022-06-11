using UnityEngine;

namespace Blockstacker.Common
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        protected virtual void Awake()
        {
            transform.parent = null;
            if (_instance == null)
            {
                _instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }

            if (_instance != this) Destroy(gameObject);
        }
    }
}