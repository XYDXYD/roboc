using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibExt;
using System;
using System.Diagnostics;
using System.IO;

namespace Network
{
	public class NetworkConnection
	{
		private NetworkPeer _owner;

		internal NetPeer _netPeer;

		private int _connectionId;

		private SendOptions[] _channels;

		private NetworkMessage _netMsg = new NetworkMessage();

		private byte[] _outBuffer;

		private bool _disposed;

		private string _address;

		public int connectionId => _connectionId;

		public bool isConnected => _netPeer != null;

		public string address => _address;

		public int hostId => _connectionId;

		internal NetworkConnection(NetworkPeer owner, NetPeer netPeer, ConnectionConfig config, ConfigData configData, int connectionId)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected I4, but got Unknown
			_owner = owner;
			_netPeer = netPeer;
			_connectionId = connectionId;
			_address = _netPeer.get_EndPoint().get_Host();
			_channels = (SendOptions[])new SendOptions[config._channels.Count];
			for (int i = 0; i < config._channels.Count; i++)
			{
				_channels[i] = (int)config._channels[i];
			}
			_outBuffer = new byte[configData.MaxPacketSize];
		}

		public void Disconnect()
		{
			_netPeer.get_NetManager().DisconnectPeer(_netPeer);
		}

		internal void HandleMessage(short msgType, NetDataReader reader, NetworkPeer peer)
		{
			if (_netPeer == null)
			{
				NetworkLogger.LogWarn("NetworkConnection.HandleMessage with null netPeer");
				return;
			}
			_netMsg.conn = this;
			_netMsg.msgType = msgType;
			if (reader != null)
			{
				_netMsg.IsReliableMsg = reader.get_IsReliableData();
				using (MemoryStream input = new MemoryStream(reader.get_Data()))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						_netMsg.reader = binaryReader;
						binaryReader.ReadInt16();
						peer.InvokeHandler(msgType, _netMsg);
					}
				}
			}
			else
			{
				_netMsg.reader = null;
				peer.InvokeHandler(msgType, _netMsg);
			}
		}

		public bool SendByChannel(short msgType, MessageBase msg, int channelId)
		{
			if (_netPeer == null)
			{
				_owner.InvokeError(this, NetworkError.WrongConnection);
				return false;
			}
			if ((uint)channelId >= _channels.Length)
			{
				_owner.InvokeError(this, NetworkError.WrongChannel);
				return false;
			}
			long numBytes;
			try
			{
				BuildMessage(writeHeader: true, msgType, msg, _outBuffer, out numBytes);
			}
			catch (NotSupportedException)
			{
				_owner.InvokeError(this, NetworkError.MessageToLong);
				return false;
			}
			catch
			{
				throw;
			}
			_netPeer.Send(_outBuffer, 0, (int)numBytes, _channels[channelId]);
			return true;
		}

		public static void BuildMessage(bool writeHeader, short msgType, MessageBase msg, byte[] buffer, out long numBytes)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					if (writeHeader)
					{
						binaryWriter.Write(msgType);
					}
					msg.GhettoSerialize(binaryWriter);
					numBytes = memoryStream.Position;
				}
			}
		}

		public void SetMaxDelay(float seconds)
		{
		}

		[Conditional("DEBUG_PACKET_IO")]
		private void DebugPacket(bool send, byte[] bytes, int numBytes)
		{
			string text = "LOCAL:" + _netPeer.get_NetManager().get_LocalEndPoint() + " REMOTE:" + _netPeer.get_EndPoint() + " " + ((!send) ? "RX" : "TX") + ": " + Common.ByteArrayToString(bytes, 0, numBytes);
			NetworkLogger.Log(text);
		}
	}
}
