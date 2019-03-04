using RCNetwork.Server;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace RCNetwork.Events
{
	internal sealed class NetworkEventManagerClientMock : INetworkEventManagerClient
	{
		private EventDispatcher _dispatcher;

		private Dictionary<NetworkEvent, Type> _events = new Dictionary<NetworkEvent, Type>();

		[Inject]
		public INetworkEventManagerServer networkEventManagerServer
		{
			private get;
			set;
		}

		public NetworkEventManagerClientMock()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			_dispatcher = new EventDispatcher();
		}

		public void RegisterEvent(NetworkEvent type, IDispatchableCommand command)
		{
			_dispatcher.Add<NetworkEvent>(type, command);
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
			networkEventManagerServer.ReceiveEvent(type, dependency.senderId, data);
		}

		public void SendEventToServerUnreliable(NetworkEvent type, NetworkDependency dependency)
		{
			SendEventToServer(type, dependency);
		}

		public void SendEventToServerExperimental(NetworkEvent type, NetworkDependency dependency)
		{
			SendEventToServer(type, dependency);
		}

		public void ReceiveEvent<T>(NetworkEvent type, T senderPlayer, byte[] data)
		{
			int senderId = -1;
			if (_events.ContainsKey(type))
			{
				if (typeof(NetworkDependency).IsAssignableFrom(_events[type]))
				{
					NetworkDependency networkDependency = Activator.CreateInstance(_events[type], data) as NetworkDependency;
					networkDependency.senderId = senderId;
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
		}

		public void ReceiveEvent<T>(NetworkEvent type, T senderPlayer, string data)
		{
			byte[] data2 = Convert.FromBase64String(data);
			ReceiveEvent(type, senderPlayer, data2);
		}
	}
}
