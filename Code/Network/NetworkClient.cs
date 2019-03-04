using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Network
{
	public class NetworkClient : NetworkPeer
	{
		private NetworkConnection _connection;

		private string _serverIp = string.Empty;

		private int _serverPort;

		private int _rtt;

		private static List<NetworkClient> s_clients = new List<NetworkClient>();

		private static bool s_active;

		public string serverIp => _serverIp;

		public int serverPort => _serverPort;

		public NetworkConnection connection => _connection;

		public bool isConnected => _connection != null;

		public static bool active => s_active;

		public static List<NetworkClient> allClients => s_clients;

		public NetworkClient()
		{
			s_active = true;
			s_clients.Add(this);
		}

		public void Connect(string serverIp, int serverPort, int outboundPort = 0)
		{
			_serverIp = serverIp;
			_serverPort = serverPort;
			if (!_manager.StartAsClient(outboundPort, serverIp, serverPort))
			{
				throw new Exception("NetManager.Start() failed");
			}
			try
			{
				_manager.Connect(serverIp, serverPort);
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode != SocketError.HostNotFound)
				{
					throw;
				}
				InvokeError(null, NetworkError.DNSFailure);
			}
			catch (Exception ex2)
			{
				if (!ex2.Message.StartsWith("Invalid address: "))
				{
					throw;
				}
				InvokeError(null, NetworkError.DNSFailure);
			}
		}

		public void Disconnect()
		{
			if (_connection != null)
			{
				_manager.DisconnectPeer(_connection._netPeer);
				_connection.Disconnect();
				_connection = null;
			}
		}

		public void Shutdown()
		{
			_manager.Stop();
			s_clients.Remove(this);
			if (s_clients.Count == 0)
			{
				s_active = false;
			}
		}

		public static void ShutdownAll()
		{
			while (s_clients.Count != 0)
			{
				s_clients[0].Shutdown();
			}
			s_clients = new List<NetworkClient>();
			s_active = false;
		}

		public bool SendByChannel(short msgType, MessageBase msg, int channelId)
		{
			return _connection.SendByChannel(msgType, msg, channelId);
		}

		internal static void UpdateClients()
		{
			for (int i = 0; i < s_clients.Count; i++)
			{
				NetworkClient networkClient = s_clients[i];
				if (networkClient != null)
				{
					NetManager manager = networkClient._manager;
					if (manager != null)
					{
						manager.PollEvents();
					}
				}
				else
				{
					s_clients.RemoveAt(i);
				}
			}
		}

		public int GetRTT()
		{
			return _rtt;
		}

		protected override NetManager CreateNetManager(ConfigData configData, int maxConnections, string connectKey, NetAuth netAuth)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			if (maxConnections > 1)
			{
				throw new ArgumentException("NetworkClient should only have one connection");
			}
			return new NetManager(this, 1, connectKey, configData, netAuth);
		}

		protected override NetworkConnection FindConnectionForNetPeer(NetPeer peer, bool assertOnError)
		{
			if (assertOnError)
			{
				if (_connection == null)
				{
					throw new Exception("NetworkClient has no connection");
				}
				if (_connection._netPeer != peer)
				{
					throw new Exception("NetworkClient peer does not match");
				}
			}
			return _connection;
		}

		protected override NetworkConnection CreateConnectionForPeer(NetPeer peer, ConnectionConfig config, ConfigData configData)
		{
			if (_connection != null)
			{
				throw new Exception("CreateConnectionForPeer when we already have a _connection");
			}
			_connection = new NetworkConnection(this, peer, config, configData, 1);
			return _connection;
		}

		protected override void RemoveConnection(NetworkConnection connection)
		{
			_connection = null;
		}

		protected override void UpdatePeerLatency(NetPeer peer, int latency)
		{
			_rtt = latency;
		}
	}
}
