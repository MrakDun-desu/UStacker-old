using System;
using System.Net;
using FishNet;
using FishNet.Transporting;
using ParrelSync;
using UnityEngine;

namespace UStacker.Multiplayer.Initialization
{
    public class MultiplayerInitializer : MonoBehaviour
    {
        [SerializeField] private NetworkDiscovery _networkDiscovery;
        [SerializeField] private GameObject _searchingForGameCanvas;
        [SerializeField] private GameObject _disconnectedCanvas;
        [SerializeField] private GameObject _lobbyCanvas;
        
        public static MultiplayerInitType InitType { get; set; }

        private void Awake()
        {
            Player.LocalPlayerStarted += OnLocalPlayerStarted;
        }

        private void OnLocalPlayerStarted(Player localPlayer)
        {
            _lobbyCanvas.SetActive(true);
        }

        private void Start()
        {
            switch (InitType)
            {
                case MultiplayerInitType.Host:
                    InstanceFinder.ServerManager.OnServerConnectionState += OnServerStateChanged;
                    InstanceFinder.ServerManager.StartConnection();
                    InstanceFinder.ClientManager.StartConnection();
                    break;
                case MultiplayerInitType.LocalClient:
                    if (ClonesManager.IsClone())
                    {
                        InstanceFinder.ClientManager.StartConnection();
                    }
                    else
                    {
                        _networkDiscovery.ServerDiscovered += OnServerDiscovered;
                        _networkDiscovery.StartSearchingForServers();
                    }
                    
                    _searchingForGameCanvas.SetActive(true);
                    InstanceFinder.ClientManager.OnClientConnectionState += OnClientStateChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnClientStateChanged(ClientConnectionStateArgs args)
        {
            switch (args.ConnectionState)
            {
                case LocalConnectionState.Stopping or LocalConnectionState.Stopped:
                    if (_disconnectedCanvas != null && _lobbyCanvas != null && _searchingForGameCanvas != null)
                    {
                        _disconnectedCanvas.SetActive(true);
                        _searchingForGameCanvas.SetActive(false);
                        _lobbyCanvas.SetActive(false);
                    }
                    break;
                case LocalConnectionState.Started:
                    _lobbyCanvas.SetActive(true);
                    _searchingForGameCanvas.SetActive(false);
                    break;
            }
        }

        private void OnDisable()
        {
            _networkDiscovery.ServerDiscovered -= OnServerDiscovered;
            Player.LocalPlayerStarted -= OnLocalPlayerStarted;
            if (InstanceFinder.ClientManager != null)
                InstanceFinder.ClientManager.OnClientConnectionState -= OnClientStateChanged;
        }

        private void OnServerStateChanged(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started)
            {
                _networkDiscovery.StartAdvertisingServer();
            }
        }

        private void OnServerDiscovered(IPEndPoint serverEndpoint)
        {
            InstanceFinder.ClientManager.StartConnection(serverEndpoint.Address.ToString(), (ushort)serverEndpoint.Port);
            _networkDiscovery.ServerDiscovered -= OnServerDiscovered;
        }
    }
}