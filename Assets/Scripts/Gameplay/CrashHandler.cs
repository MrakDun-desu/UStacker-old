using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UStacker.Gameplay.Communication;
using Logger = UStacker.Common.Logger;

namespace UStacker.Gameplay
{
    public class CrashHandler : MonoBehaviour
    {
        [SerializeField] private Mediator _mediator;
        [SerializeField] private GameObject _crashCanvas;
        [SerializeField] private Button _exitButton;
        [SerializeField] private TMP_Text _crashMessage;
        [SerializeField] private TMP_Text _crashCountdown;
        [Range(10, 50)] [SerializeField] private uint _crashCountdownCount = 20u;

        private void Awake()
        {
            _exitButton.onClick.AddListener(() => SceneManager.LoadScene("Scene_Menu_Main"));
        }

        private void OnEnable()
        {
            _mediator.Register<GameCrashedMessage>(CrashGame);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GameCrashedMessage>(CrashGame);
        }

        private void CrashGame(GameCrashedMessage message)
        {
            Logger.Log("CRASH: " + message.CrashMessage);
            StartCoroutine(CrashGameCor(message.CrashMessage));
        }

        private IEnumerator CrashGameCor(string crashMessage)
        {
            _crashCanvas.SetActive(true);
            _exitButton.Select();

            for (var countdown = _crashCountdownCount; countdown > 0; countdown--)
            {
                _crashMessage.text = crashMessage;
                _crashCountdown.text = $"Exiting in {countdown}...";
                yield return new WaitForSeconds(1);
            }

            SceneManager.LoadScene("Scene_Menu_Main");
        }

    }
}