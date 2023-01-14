using System;
using System.Linq;
using UStacker.GlobalSettings.Appliers;
using UStacker.GlobalSettings.Changers;
using UStacker.GlobalSettings.StatCounting;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UStacker.Common;

namespace UStacker.GlobalSettings.Startup
{
    public class StartupHandler : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private InputActionAsset _actionAsset;
        [SerializeField] private TMP_Text _loaderMessagePrefab;
        [SerializeField] private Transform _loaderMessageParent;
        [SerializeField] private PremadeStatCountersSo _premadeStatCounters;
        [SerializeField] private StringReferenceSO[] _gameTypes = Array.Empty<StringReferenceSO>();

        private uint _loadersActive;

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
#if !UNITY_EDITOR
            var collectorManager = new GameObject("GarbageCollectorManager");
            collectorManager.AddComponent<GarbageCollectorManager>();
#endif
            AppSettings.TryLoad();
            SettingChanged?.Invoke();
            AddDefaultStatCounters();
            FinishStartup();
        }

        public event Action SettingChanged;

        private void FinishStartup()
        {
            if (_loadersActive == 0)
                SceneManager.LoadScene("Scene_Menu_Main");
        }

        private void AddSceneChangeMethods()
        {
            SceneManager.sceneLoaded += (_, _) => _actionAsset.LoadBindingOverridesFromJson(AppSettings.Rebinds);
            SceneManager.sceneLoaded += (_, _) => DOTween.KillAll();
        }

        private void RemoveLoader()
        {
            _loadersActive--;
            FinishStartup();
        }

        private void AddDefaultStatCounters()
        {
            if (AppSettings.StatCounting.StatCounterGroups.Count > 0 || _premadeStatCounters == null) return;
            AppSettings.StatCounting.DefaultGroup = _premadeStatCounters.DefaultGroup;

            foreach (var group in _premadeStatCounters.PremadeGroups)
            {
                var groupId = Guid.NewGuid();
                while (AppSettings.StatCounting.StatCounterGroups.ContainsKey(groupId))
                    groupId = Guid.NewGuid();

                AppSettings.StatCounting.StatCounterGroups[groupId] = group;

                if (_gameTypes.Select(gameType => gameType.Value).Contains(group.Name))
                    AppSettings.StatCounting.GameStatCounterDictionary[group.Name] = groupId;
            }

        }
    }
}