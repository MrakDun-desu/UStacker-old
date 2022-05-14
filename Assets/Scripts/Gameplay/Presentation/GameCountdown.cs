using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Presentation
{
    [RequireComponent(typeof(TMP_Text))]
    public class GameCountdown : MonoBehaviour
    {
        public float CountdownInterval = .1f;
        public uint CountdownCount = 3;
        [SerializeField] private string _lastMessage = "Start!";
        [SerializeField] private UnityEvent CountdownFinished;

        private TMP_Text _countdownText;

        private void Awake()
        {
            _countdownText = GetComponent<TMP_Text>();
        }

        public void StartCountdown()
        {
            StartCoroutine(CorStartCountdown());
        }

        private IEnumerator CorStartCountdown()
        {
            for (var count = CountdownCount; count > 0; count--)
            {
                _countdownText.text = count.ToString();
                yield return new WaitForSeconds(CountdownInterval);
            }

            _countdownText.text = _lastMessage;
            yield return new WaitForSeconds(CountdownInterval);
            CountdownFinished.Invoke();
        }
        
        
    }
}