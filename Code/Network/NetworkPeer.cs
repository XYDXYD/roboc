using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibExt;
using System;
using System.Collections.Generic;
using System.IO;

namespace Network
{
	public abstract class NetworkPeer : INetEventListener
	{
		public delegate void NetworkMessageDelegate(NetworkMessage netMsg);

		private const string ConnectKey = "Robocraft";

		protected NetManager _manager;

		private ConnectionConfig _config;

		private ConfigData _configData;

		private Dictionary<short, NetworkMessageDelegate> _handlers = new Dictionary<short, NetworkMessageDelegate>();

		public bool Configure(ConnectionConfig config, ConfigData configData, int maxConnections, string customAuthString = null)
		{
			_config = config;
			_configData = configData;
			if (configData.AuthInfo == null)
			{
				throw new ArgumentNullException("config.AuthInfo null");
			}
			using (MemoryStream input = new MemoryStream(configData.AuthInfo))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					NetEncryptionFactory val = null;
					NetEncryptionParams val2 = null;
					bool flag = binaryReader.ReadByte() == 1;
					if (flag)
					{
						val = new PacketWrapNetEncryptionFactory(configData.PacketSize);
						val2 = new PacketWrapNetEncryptionParams();
						val2.Deserialize(binaryReader);
					}
					NetAuth val3;
					if (binaryReader.ReadBoolean())
					{
						NetworkLogger.Log("[x_auth] auth" + ((!flag) ? string.Empty : " & encr"));
						val3 = CreateCustomNetAuth(val, val2, binaryReader);
						val3.set_CustomAuthString(customAuthString);
					}
					else if (val != null)
					{
						NetworkLogger.Log("[x_auth] encr");
						val3 = new OneKeyNetAuth(val, val2);
					}
					else
					{
						NetworkLogger.Log("[x_auth] no auth or encr");
						val3 = null;
					}
					_manager = CreateNetManager(configData, maxConnections, "Robocraft", val3);
				}
			}
			_manager.set_UpdateTime(configData.NetworkPeerUpdateInterval);
			_manager.MergeEnabled = true;
			_manager.MaxSendPacketsPerUpdate = 100;
			return true;
		}

		public void InvokeError(NetworkConnection connection, NetworkError error)
		{
			ErrorMessage errorMessage = new ErrorMessage();
			errorMessage.errorCode = (int)error;
			byte[] buffer = new byte[16];
			NetworkConnection.BuildMessage(writeHeader: false, 0, errorMessage, buffer, out long _);
			Stream input = new MemoryStream(buffer);
			NetworkMessage networkMessage = new NetworkMessage();
			networkMessage.conn = connection;
			networkMessage.msgType = 34;
			networkMessage.reader = new BinaryReader(input);
			InvokeHandler(34, networkMessage);
		}

		void INetEventListener.OnPeerConnected(NetPeer peer)
		{
			NetworkLogger.Log("NetworkPeer.OnPeerConnected LOCAL:" + peer.get_NetManager().get_LocalEndPoint() + " REMOTE:" + peer.get_EndPoint());
			NetworkConnection networkConnection = CreateConnectionForPeer(peer, _config, _configData);
			networkConnection.HandleMessage(32, null, this);
		}

		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected I4, but got Unknown
			NetworkLogger.Log("NetworkPeer.OnPeerDisconnected " + peer.get_EndPoint() + " reason:" + ((object)disconnectInfo.Reason).ToString());
			NetworkConnection networkConnection = FindConnectionForNetPeer(peer, assertOnError: false);
			NetworkError networkError = NetworkError.Ok;
			DisconnectReason reason = disconnectInfo.Reason;
			switch ((int)reason)
			{
			case 1:
				networkError = NetworkError.VersionMismatch;
				break;
			case 2:
				networkError = NetworkError.CRCMismatch;
				break;
			case 0:
				networkError = NetworkError.DNSFailure;
				break;
			case 4:
				networkError = NetworkError.DisconnectedByReceieveError;
				break;
			case 3:
				networkError = NetworkError.Timeout;
				break;
			}
			if (networkError != 0)
			{
				InvokeError(networkConnection, networkError);
			}
			if (networkConnection != null)
			{
				networkConnection.HandleMessage(33, null, this);
				RemoveConnection(networkConnection);
			}
		}

		void INetEventListener.OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
		{
			NetworkError error;
			switch (socketErrorCode)
			{
			case 10043:
				error = NetworkError.VersionMismatch;
				break;
			case 11001:
				error = NetworkError.DNSFailure;
				break;
			default:
				error = (NetworkError)(-socketErrorCode);
				break;
			}
			NetworkLogger.Log("NetworkPeer.OnNetworkError(endPoint=" + ((object)endPoint).ToString() + ", error=" + error.ToString() + ")");
			InvokeError(null, error);
		}

		void INetEventListener.OnNetworkReceive(NetPeer peer, NetDataReader reader)
		{
			NetworkConnection networkConnection = FindConnectionForNetPeer(peer, assertOnError: true);
			short @short = reader.GetShort();
			networkConnection.HandleMessage(@short, reader, this);
		}

		void INetEventListener.OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
		{
			throw new NotImplementedException();
		}

		void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
		{
			UpdatePeerLatency(peer, latency);
		}

		protected virtual NetAuth CreateCustomNetAuth(NetEncryptionFactory encrFactory, NetEncryptionParams encrParams, BinaryReader reader)
		{
			throw new NotImplementedException();
		}

		protected abstract NetManager CreateNetManager(ConfigData configData, int maxConnections, string connectKey, NetAuth netAuth);

		protected abstract NetworkConnection FindConnectionForNetPeer(NetPeer peer, bool assertOnError);

		protected abstract NetworkConnection CreateConnectionForPeer(NetPeer peer, ConnectionConfig config, ConfigData configData);

		protected abstract void RemoveConnection(NetworkConnection connection);

		protected virtual void UpdatePeerLatency(NetPeer peer, int latency)
		{
		}

		public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
		{
			_handlers[msgType] = handler;
		}

		public void UnregisterHandler(short msgType)
		{
			_handlers.Remove(msgType);
		}

		public void InvokeHandler(short msgType, NetworkMessage netMsg)
		{
			if (_handlers.TryGetValue(msgType, out NetworkMessageDelegate value))
			{
				value(netMsg);
			}
		}
	}
}
