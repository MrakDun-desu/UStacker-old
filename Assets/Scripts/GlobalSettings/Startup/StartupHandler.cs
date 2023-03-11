using System;
using System.IO;
using System.Threading.Tasks;
using UStacker.GlobalSettings.Appliers;
using UStacker.GlobalSettings.Changers;
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

        private uint _loadersActive;
        public event Action SettingChanged;

        private void Start()
        {
            if (!Directory.Exists(PersistentPaths.DataPath))
                FileHandling.CreateDirectoriesRecursively(PersistentPaths.DataPath);
            
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
                    DecrementLoaderCount();
                });
            }

            AddSceneChangeMethods();
            var settingsSaver = new GameObject("OnQuitSettingsSaver");
            settingsSaver.AddComponent<OnQuitSettingsSaver>();
            _ = LoadAppSettings();
        }

        private async Task LoadAppSettings()
        {
            await AppSettings.TryLoadAsync();
            SettingChanged?.Invoke();
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
            SceneManager.sceneLoaded += (_, _) => DOTween.KillAll();
        }

        private void DecrementLoaderCount()
        {
            _loadersActive--;
            FinishStartup();
        }
    }
}