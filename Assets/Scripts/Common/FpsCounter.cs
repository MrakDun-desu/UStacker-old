using System;
using System.Collections;
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

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            StartCoroutine(ShowFpsCor());
        }

        private IEnumerator ShowFpsCor()
        {
            while (true)
            {
                _text.text = Math.Round(1 / Time.deltaTime).ToString(CultureInfo.InvariantCulture);
                yield return new WaitForSeconds(_interval);
            }
        }
    }
}