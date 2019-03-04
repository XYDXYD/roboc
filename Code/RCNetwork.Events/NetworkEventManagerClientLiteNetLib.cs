using Network;
using RCNetwork.UNet.Client;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace RCNetwork.Events
{
	internal sealed class NetworkEventManagerClientLiteNetLib : INetworkEventManagerClient
	{
		private EventDispatcher _dispatcher;

		private Dictionary<NetworkEvent, Type> _events = new Dictionary<NetworkEvent, Type>();

		private bool _unreliableMessagesEnabled;

		[Inject]
		public NetworkClientPool playerManager
		{
			private get;
			set;
		}

		internal EventRouterClientLiteNetLib remoteSender
		{
			private get;
			set;
		}

		public NetworkEventManagerClientLiteNetLib()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			_dispatcher = new EventDispatcher();
			ClientConfigData.TryGetValue("UnreliableMessages", out string value);
			_unreliableMessagesEnabled = (value == "true");
		}

		public void RegisterEvent(NetworkEvent type, IDispatchableCommand command)
		{
			if (type == NetworkEvent.BroadcastLoadingProgress)
			{
				Debug.LogError((object)"RegisterEvent");
			}
			_dispatcher.Add<NetworkEvent>(type, command);
			if (_events.ContainsKey(type))
			{
				_events[type] = null;
			}
			else
			{
				_events.Add(type, null);
			}
		}

		public void RegisterEvent<T>(NetworkEvent type, IDispatchableCommand command) where T : NetworkDependency
		{
			_dispatcher.Add<NetworkEvent>(type, command);
			if (_events.ContainsKey(type))
			{
				_events[type] = typeof(T);
			}
			else
			{
				_events.Add(type, typeof(T));
			}
		}

		public void SendEventToServer(NetworkEvent type, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			remoteSender.SendRemoteEvent(type, data);
		}

		public void SendEventToServerUnreliable(NetworkEvent type, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			remoteSender.SendUnreliableRemoteEvent(type, data);
		}

		public void SendEventToServerExperimental(NetworkEvent type, NetworkDependency dependency)
		{
			if (_unreliableMessagesEnabled)
			{
				SendEventToServerUnreliable(type, dependency);
			}
			else
			{
				SendEventToServer(type, dependency);
			}
		}

		public void ReceiveEvent<T>(NetworkEvent type, T senderPlayer, byte[] data)
		{
			if (!_events.ContainsKey(type))
			{
				return;
			}
			if (typeof(NetworkDependency).IsAssignableFrom(_events[type]))
			{
				NetworkDependency networkDependency = Activator.CreateInstance(_events[type], data) as NetworkDependency;
				if (!playerManager.TryGetPlayerId(senderPlayer, out networkDependency.senderId))
				{
					networkDependency.senderId = -1;
				}
				_dispatcher.Dispatch<NetworkEvent>(type, new object[1]
				{
					networkDependency
				});
			}
			else
			{
				_dispatcher.Dispatch<NetworkEvent>(type);
			}
		}

		internal void ReceiveEvent(NetworkMessage msg, NetworkConnection conn)
		{
			if (msg.type == 146)
			{
				Console.Log("Consuming '" + (NetworkEvent)msg.type + "' event.");
			}
			ReceiveEvent((NetworkEvent)msg.type, conn, msg.data);
		}

		internal bool CanConsumeEvent(NetworkMessage msg)
		{
			NetworkEvent type = (NetworkEvent)msg.type;
			return _events.ContainsKey(type);
		}

		internal bool IsClearToken(NetworkMessage msg)
		{
			NetworkEvent type = (NetworkEvent)msg.type;
			return NetworkEventHints.IsClearToken(type);
		}
	}
}
