using System;
using Blockstacker.GlobalSettings.Appliers;
using Blockstacker.GlobalSettings.Changers;
using Blockstacker.GlobalSettings.StatCounting;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Blockstacker.GlobalSettings.Startup
{
    public class StartupHandler : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private InputActionAsset _actionAsset;
        [SerializeField] private TMP_Text _loaderMessagePrefab;
        [SerializeField] private Transform _loaderMessageParent;
        [SerializeField] private PremadeStatCountersSo _premadeStatCounters;

        private uint _loadersActive;
        public event Action SettingChanged;

        private void Start()
        {
            foreach (var loader in GetComponents<IAsyncApplier>())
            {
                TMP_Text messageText = null;
                _loadersActive++;
                loader.LoadingStarted.AddListener(() =>
                {
                    messageText = Instantiate(_loaderMessagePrefab, _loaderMessageParent);
                    messageText.text = loader.OngoingMessage;
                });
                loader.LoadingFinished.AddListener(() =>
                {
                    if (messageText != null)
                        messageText.gameObject.SetActive(false);
                    RemoveLoader();
                });
            }

            AddSceneChangeMethods();
            var settingsSaver = new GameObject("OnQuitSettingsSaver");
            settingsSaver.AddComponent<OnQuitSettingsSaver>();
            AppSettings.TryLoad();
            SettingChanged?.Invoke();
            if (AppSettings.StatCounting.StatCounterGroups.Count <= 0 && _premadeStatCounters != null)
            {
                AppSettings.StatCounting.DefaultGroup = _premadeStatCounters.DefaultGroup;

                foreach (var group in _premadeStatCounters.PremadeGroups)
                    while (!AppSettings.StatCounting.StatCounterGroups.TryAdd(Guid.NewGuid(), group))
                    {
                    }
            }

            FinishStartup();
        }

        private void FinishStartup()
        {
            if (_loadersActive == 0)
                SceneManager.LoadScene("Scene_Menu_Main");
        }

        private void AddSceneChangeMethods()
        {
            SceneManager.sceneLoaded += (_, _) => _actionAsset.LoadBindingOverridesFromJson(AppSettings.Rebinds);
        }

        private void RemoveLoader()
        {
            _loadersActive--;
            FinishStartup();
        }
    }
}