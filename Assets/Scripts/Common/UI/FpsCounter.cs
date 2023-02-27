using System.Collections;
using TMPro;
using UnityEngine;

namespace UStacker.Common.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private float _interval = 1f;

        private TMP_Text _text;

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
                _text.text = $"FPS: {Mathf.RoundToInt(1/Time.deltaTime)}";
                yield return new WaitForSeconds(_interval);
            }
        }
    }
}