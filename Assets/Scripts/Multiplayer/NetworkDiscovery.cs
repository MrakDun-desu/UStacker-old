using FishNet.Managing;
using FishNet.Managing.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FishNet;
using UnityEngine;

namespace UStacker.Multiplayer
{
	public sealed class NetworkDiscovery : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("A string that differentiates application/game from others. Must not be null, empty, or blank. Do not change during runtime")]
		private string _secret;

		[SerializeField]
		[Tooltip("The port number used by this NetworkDiscovery component. Must be different from the one used by the Transport")]
		private ushort _port;

		[SerializeField]
		[Tooltip("Interval between discovery tries in seconds")]
		private float _discoveryInterval;

		private UdpClient _serverUdpClient;
		private UdpClient _clientUdpClient;
		private byte[] _secretBytes;

		public bool IsAdvertising => _serverUdpClient != null;

		public bool IsSearching => _clientUdpClient != null;

		public event Action<IPEndPoint> ServerDiscovered;

		private void Awake()
		{
			_secretBytes = Encoding.UTF8.GetBytes(_secret);
		}

		private void OnDestroy()
		{
			StopAdvertisingServer();
			StopSearchingForServers();
		}

		public void StartAdvertisingServer()
		{
			if (!InstanceFinder.IsServer)
			{
				if (NetworkManager.StaticCanLog(LoggingType.Warning)) Debug.LogWarning("Unable to start advertising server. Server is inactive.", this);

				return;
			}

			if (IsAdvertising)
			{
				if (NetworkManager.StaticCanLog(LoggingType.Common)) Debug.Log("Server is already being advertised.", this);

				return;
			}

			if (_port == InstanceFinder.TransportManager.Transport.GetPort())
			{
				if (NetworkManager.StaticCanLog(LoggingType.Warning)) Debug.LogWarning("Unable to start advertising server on the same port as the transport.", this);

				return;
			}

			_serverUdpClient = new UdpClient(_port)
			{
				EnableBroadcast = true,
				MulticastLoopback = false,
				Ttl = 10
			};

			_ = AdvertiseServerAsync();

			if (NetworkManager.StaticCanLog(LoggingType.Common)) Debug.Log("Started advertising server.", this);
		}

		public void StopAdvertisingServer()
		{
			if (_serverUdpClient == null) return;

			_serverUdpClient.Close();

			_serverUdpClient = null;

			if (NetworkManager.StaticCanLog(LoggingType.Common)) Debug.Log("Stopped advertising server.", this);
		}

		private async Task AdvertiseServerAsync()
		{
			while (_serverUdpClient != null)
			{
				var result = await _serverUdpClient.ReceiveAsync();
				

				var receivedSecret = Encoding.UTF8.GetString(result.Buffer);
				Debug.Log("Received message from client with content " + receivedSecret);

				if (receivedSecret != _secret)
				{
					await Task.Delay(TimeSpan.FromSeconds(_discoveryInterval));
					continue;
				}
				var okBytes = BitConverter.GetBytes(true);

				await _serverUdpClient.SendAsync(okBytes, okBytes.Length, result.RemoteEndPoint);
			}
		}

		public void StartSearchingForServers()
		{
			if (InstanceFinder.IsServer)
			{
				if (NetworkManager.StaticCanLog(LoggingType.Warning)) Debug.LogWarning("Unable to start searching for servers. Server is active.", this);

				return;
			}

			if (InstanceFinder.IsClient)
			{
				if (NetworkManager.StaticCanLog(LoggingType.Warning)) Debug.LogWarning("Unable to start searching for servers. Client is active.", this);

				return;
			}

			if (IsSearching)
			{
				if (NetworkManager.StaticCanLog(LoggingType.Common)) Debug.Log("Already searching for servers.", this);

				return;
			}

			_clientUdpClient = new UdpClient
			{
				EnableBroadcast = true,
				MulticastLoopback = false,
			};

			_ = SearchForServersAsync();

			if (NetworkManager.StaticCanLog(LoggingType.Common)) Debug.Log("Started searching for servers.", this);
		}

		public void StopSearchingForServers()
		{
			if (_clientUdpClient == null) return;

			_clientUdpClient.Close();

			_clientUdpClient = null;

			if (NetworkManager.StaticCanLog(LoggingType.Common)) Debug.Log("Stopped searching for servers.", this);
		}

		private async Task SearchForServersAsync()
		{
			var endPoint = new IPEndPoint(IPAddress.Broadcast, _port);

			while (_clientUdpClient != null)
			{
				await _clientUdpClient.SendAsync(_secretBytes, _secretBytes.Length, endPoint);
				
				Debug.Log("Sent search message");

				var result = await _clientUdpClient.ReceiveAsync();
				
				Debug.Log("Received search result");

				if (!BitConverter.ToBoolean(result.Buffer, 0))
				{
					await Task.Delay(TimeSpan.FromSeconds(_discoveryInterval));
					continue;
				}
				ServerDiscovered?.Invoke(result.RemoteEndPoint);

				StopSearchingForServers();
			}
		}
	}
}
