using UnityEngine;

namespace UStacker.Common
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T _instance;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T) this;
                DontDestroyOnLoad(gameObject);
            }

            if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (transform is RectTransform rectTransform)
                rectTransform.SetParent(null, false);
            else
                transform.SetParent(null, true);
        }
    }
}