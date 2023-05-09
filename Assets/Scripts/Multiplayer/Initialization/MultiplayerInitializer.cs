
/************************************
MultiplayerInitializer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Linq;
using System.Net;
using FishNet;
using FishNet.Transporting;
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
                    _networkDiscovery.ServerDiscovered += OnServerDiscovered;
                    _networkDiscovery.StartSearchingForServers();
                    _searchingForGameCanvas.SetActive(true);
                    InstanceFinder.ClientManager.OnClientConnectionState += OnClientStateChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisable()
        {
            _networkDiscovery.ServerDiscovered -= OnServerDiscovered;
            Player.LocalPlayerStarted -= OnLocalPlayerStarted;
            if (InstanceFinder.ClientManager != null)
                InstanceFinder.ClientManager.OnClientConnectionState -= OnClientStateChanged;
        }

        private void OnLocalPlayerStarted(Player localPlayer)
        {
            _lobbyCanvas.SetActive(true);
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

        private void OnServerStateChanged(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started) _networkDiscovery.StartAdvertisingServer();
        }

        private void OnServerDiscovered(IPEndPoint serverEndpoint)
        {
            var address = serverEndpoint.Address.ToString();
            if (Dns.GetHostEntry(Dns.GetHostName()).AddressList.Contains(serverEndpoint.Address))
                address = "localhost";

            InstanceFinder.ClientManager.StartConnection(address);
            _networkDiscovery.ServerDiscovered -= OnServerDiscovered;
        }
    }
}
/************************************
end MultiplayerInitializer.cs
*************************************/
