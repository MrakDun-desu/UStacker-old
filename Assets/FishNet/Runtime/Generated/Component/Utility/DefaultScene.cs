﻿using System.IO;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using FishNet.Utility;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

/// <summary>
///     Add to a NetworkManager object to change between Online and Offline scene based on connection states of the server
///     or client.
/// </summary>
[AddComponentMenu("FishNet/Component/DefaultScene")]
public class DefaultScene : MonoBehaviour
{
    #region Private.

    /// <summary>
    ///     NetworkManager for this component.
    /// </summary>
    private NetworkManager _networkManager;

    #endregion

    private void Awake()
    {
        InitializeOnce();
    }

    private void OnDestroy()
    {
        if (!ApplicationState.IsQuitting() && _networkManager != null && _networkManager.Initialized)
        {
            _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
            _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
            _networkManager.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
            _networkManager.ServerManager.OnAuthenticationResult -= ServerManager_OnAuthenticationResult;
        }
    }

    /// <summary>
    ///     Initializes this script for use.
    /// </summary>
    private void InitializeOnce()
    {
        _networkManager = GetComponentInParent<NetworkManager>();
        if (_networkManager == null)
        {
            NetworkManager.StaticLogError(
                $"NetworkManager not found on {gameObject.name} or any parent objects. DefaultScene will not work.");
            return;
        }

        //A NetworkManager won't be initialized if it's being destroyed.
        if (!_networkManager.Initialized)
            return;
        if (_onlineScene == string.Empty || _offlineScene == string.Empty)
        {
            NetworkManager.StaticLogWarning("Online or Offline scene is not specified. Default scenes will not load.");
            return;
        }

        _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
        _networkManager.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        _networkManager.ServerManager.OnAuthenticationResult += ServerManager_OnAuthenticationResult;
        if (_startInOffline)
            LoadOfflineScene();
    }

    /// <summary>
    ///     Called when a scene load ends.
    /// </summary>
    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
    {
        var onlineLoaded = false;
        foreach (var s in obj.LoadedScenes)
            if (s.name == GetSceneName(_onlineScene))
            {
                onlineLoaded = true;
                break;
            }

        //If online scene was loaded then unload offline.
        if (onlineLoaded)
            UnloadOfflineScene();
    }

    /// <summary>
    ///     Called after the local server connection state changes.
    /// </summary>
    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        /* When server starts load online scene as global.
         * Since this is a global scene clients will automatically
         * join it when connecting. */
        if (obj.ConnectionState == LocalConnectionState.Started)
        {
            /* If not exactly one server is started then
             * that means either none are started, which isnt true because
             * we just got a started callback, or two+ are started.
             * When a server has already started there's no reason to load
             * scenes again. */
            if (!_networkManager.ServerManager.OneServerStarted())
                return;

            //If here can load scene.
            var sld = new SceneLoadData(GetSceneName(_onlineScene));
            sld.ReplaceScenes = _replaceScenes;
            if (_useGlobalScenes)
                _networkManager.SceneManager.LoadGlobalScenes(sld);
            else
                _networkManager.SceneManager.LoadConnectionScenes(sld);
        }
        //When server stops load offline scene.
        else if (obj.ConnectionState == LocalConnectionState.Stopped)
        {
            LoadOfflineScene();
        }
    }

    /// <summary>
    ///     Called after the local client connection state changes.
    /// </summary>
    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Stopped)
            //Only load offline scene if not also server.
            if (!_networkManager.IsServer)
                LoadOfflineScene();
    }

    /// <summary>
    ///     Called when a client completes authentication.
    /// </summary>
    private void ServerManager_OnAuthenticationResult(NetworkConnection arg1, bool authenticated)
    {
        /* This is only for loading connection scenes.
         * If using global there is no need to continue. */
        if (_useGlobalScenes)
            return;
        if (!authenticated)
            return;

        var sld = new SceneLoadData(GetSceneName(_onlineScene));
        _networkManager.SceneManager.LoadConnectionScenes(arg1, sld);
    }


    /// <summary>
    ///     Loads offlineScene as single.
    /// </summary>
    private void LoadOfflineScene()
    {
        //Already in offline scene.
        if (UnitySceneManager.GetActiveScene().name == GetSceneName(_offlineScene))
            return;
        //Only use scene manager if networking scenes. I may add something in later to do both local and networked.
        UnitySceneManager.LoadScene(_offlineScene);
    }

    /// <summary>
    ///     Unloads the offline scene.
    /// </summary>
    private void UnloadOfflineScene()
    {
        var s = UnitySceneManager.GetSceneByName(GetSceneName(_offlineScene));
        if (string.IsNullOrEmpty(s.name))
            return;

        UnitySceneManager.UnloadSceneAsync(s);
    }

    /// <summary>
    ///     Returns a scene name from fullPath.
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    private string GetSceneName(string fullPath)
    {
        return Path.GetFileNameWithoutExtension(fullPath);
    }

    #region Serialized.

    [Tooltip("True to load the online scene as global, false to load it as connection.")] [SerializeField]
    private bool _useGlobalScenes = true;

    /// <summary>
    ///     True to replace all scenes with the offline scene immediately.
    /// </summary>
    [Tooltip("True to replace all scenes with the offline scene immediately.")] [SerializeField]
    private bool _startInOffline;

    /// <summary>
    /// </summary>
    [Tooltip("Scene to load when disconnected. Server and client will load this scene.")] [SerializeField] [Scene]
    private string _offlineScene;

    /// <summary>
    ///     Sets which offline scene to use.
    /// </summary>
    /// <param name="sceneName">Scene name to use as the offline scene.</param>
    public void SetOfflineScene(string sceneName)
    {
        _offlineScene = sceneName;
    }

    /// <summary>
    ///     Scene to load when disconnected. Server and client will load this scene.
    /// </summary>
    /// <returns></returns>
    public string GetOfflineScene()
    {
        return _offlineScene;
    }

    /// <summary>
    /// </summary>
    [Tooltip("Scene to load when connected. Server and client will load this scene.")] [SerializeField] [Scene]
    private string _onlineScene;

    /// <summary>
    ///     Sets which online scene to use.
    /// </summary>
    /// <param name="sceneName">Scene name to use as the online scene.</param>
    public void SetOnlineScene(string sceneName)
    {
        _onlineScene = sceneName;
    }

    /// <summary>
    ///     Scene to load when connected. Server and client will load this scene.
    /// </summary>
    /// <returns></returns>
    public string GetOnlineScene()
    {
        return _onlineScene;
    }

    /// <summary>
    ///     Which scenes to replace when loading into OnlineScene.
    /// </summary>
    [Tooltip("Which scenes to replace when loading into OnlineScene.")] [SerializeField]
    private ReplaceOption _replaceScenes = ReplaceOption.All;

    #endregion
}