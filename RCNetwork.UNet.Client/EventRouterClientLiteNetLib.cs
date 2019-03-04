using Network;
using RCNetwork.Events;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace RCNetwork.UNet.Client
{
	internal sealed class EventRouterClientLiteNetLib
	{
		private struct BufferEntry
		{
			public NetworkMessage msg;

			public NetworkConnection conn;

			public BufferEntry(NetworkMessage pMsg, NetworkConnection pConn)
			{
				msg = pMsg;
				conn = pConn;
			}
		}

		private static Queue<BufferEntry> _receivedMessagesQueue = new Queue<BufferEntry>();

		private NetworkMessage _msg = new NetworkMessage();

		private NetworkClient _networkClient;

		private NetworkEventManagerClientLiteNetLib _networkEventManager;

		public EventRouterClientLiteNetLib(NetworkClient networkClient, NetworkEventManagerClientLiteNetLib networkEventManager)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			_networkEventManager = networkEventManager;
			_networkClient = networkClient;
			_networkClient.RegisterHandler(NetworkMsgType.ServerMsg, OnServerMessageReceived);
			_networkEventManager.remoteSender = this;
			TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator((IEnumerator)new LoopActionEnumerator((Action)TryProcessBufferedIncomingMessages))
				.Start((Action<PausableTaskException>)null, (Action)null);
		}

		public void SendRemoteEvent(NetworkEvent type, byte[] data)
		{
			if (_networkClient.isConnected)
			{
				_msg.type = (short)type;
				_msg.data = data;
				_networkClient.SendByChannel(NetworkMsgType.ClientMsg, _msg, 0);
			}
		}

		public void SendUnreliableRemoteEvent(NetworkEvent type, byte[] data)
		{
			if (_networkClient.isConnected)
			{
				_msg.type = (short)type;
				_msg.data = data;
				_networkClient.SendByChannel(NetworkMsgType.ClientMsg, _msg, 1);
			}
		}

		internal static void ClearBufferOnDisconnect()
		{
			_receivedMessagesQueue.Clear();
		}

		private void OnServerMessageReceived(Network.NetworkMessage netMsg)
		{
			NetworkMessage networkMessage = netMsg.ReadMessage<NetworkMessage>();
			if (_networkEventManager.IsClearToken(networkMessage))
			{
				Console.Log("Received ClearToken message " + (NetworkEvent)networkMessage.type);
				_receivedMessagesQueue.Clear();
			}
			TryProcessMessage(networkMessage, netMsg);
		}

		private void TryProcessMessage(NetworkMessage msg, Network.NetworkMessage netMsg)
		{
			if (netMsg.IsReliableMsg)
			{
				TryProcessBufferedIncomingMessages();
				if (_receivedMessagesQueue.Count == 0 && _networkEventManager.CanConsumeEvent(msg))
				{
					_networkEventManager.ReceiveEvent(msg, netMsg.conn);
					return;
				}
				_receivedMessagesQueue.Enqueue(new BufferEntry(msg, netMsg.conn));
				Console.Log("Buffering network message " + (NetworkEvent)msg.type);
			}
			else if (_networkEventManager.CanConsumeEvent(msg))
			{
				_networkEventManager.ReceiveEvent(msg, netMsg.conn);
			}
			else
			{
				Console.LogWarning("Dropping unreliable message of type '" + (NetworkEvent)msg.type + "'.");
			}
		}

		private void TryProcessBufferedIncomingMessages()
		{
			while (_receivedMessagesQueue.Count != 0)
			{
				BufferEntry bufferEntry = _receivedMessagesQueue.Peek();
				if (_networkEventManager.CanConsumeEvent(bufferEntry.msg))
				{
					_receivedMessagesQueue.Dequeue();
					_networkEventManager.ReceiveEvent(bufferEntry.msg, bufferEntry.conn);
					continue;
				}
				break;
			}
		}
	}
}
