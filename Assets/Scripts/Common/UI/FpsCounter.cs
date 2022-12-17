using System.Collections;
using TMPro;
using UnityEngine;

namespace Blockstacker.Common.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private float _interval = 1f;
        private float _currentFps;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            StartCoroutine(ShowFpsCor());
        }

        private void Update()
        {
            _currentFps = 1 / Time.deltaTime;
        }

        private IEnumerator ShowFpsCor()
        {
            while (true)
            {
                _text.text = $"FPS: {_currentFps}";
                yield return new WaitForSeconds(_interval);
            }
        }
    }
}