using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Blockstacker.Common
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private float _interval = 1f;

        private TMP_Text _text;
        private float _currentFps;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            StartCoroutine(ShowFpsCor());
        }

        private IEnumerator ShowFpsCor()
        {
            while (true)
            {
                _text.text = $"FPS: {_currentFps}";
                yield return new WaitForSeconds(_interval);
            }
        }

        private void Update()
        {
            _currentFps = 1 / Time.deltaTime;
        }
    }
}