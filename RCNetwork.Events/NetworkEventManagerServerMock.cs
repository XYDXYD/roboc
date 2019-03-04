using RCNetwork.Server;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace RCNetwork.Events
{
	internal sealed class NetworkEventManagerServerMock : INetworkEventManagerServer
	{
		private EventDispatcher _dispatcher;

		private const int _selfId = -1;

		private Dictionary<NetworkEvent, Type> _events = new Dictionary<NetworkEvent, Type>();

		[Inject]
		public INetworkEventManagerClient networkEventManagerClient
		{
			private get;
			set;
		}

		public NetworkEventManagerServerMock()
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

		public void SendEventToPlayer(NetworkEvent type, int player, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			networkEventManagerClient.ReceiveEvent(type, -1, data);
		}

		public void SendEventToPlayers(NetworkEvent type, int[] players, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			networkEventManagerClient.ReceiveEvent(type, -1, data);
		}

		public void BroadcastEventToAllPlayers(NetworkEvent type, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			networkEventManagerClient.ReceiveEvent(type, -1, data);
		}

		public void BroadcastEventToAllPlayersExcept(NetworkEvent type, int playerId, NetworkDependency dependency)
		{
			if (playerId != -1)
			{
				byte[] data = dependency.Serialise();
				networkEventManagerClient.ReceiveEvent(type, -1, data);
			}
		}

		public void SendEventToPlayerUnreliable(NetworkEvent type, int player, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			networkEventManagerClient.ReceiveEvent(type, -1, data);
		}

		public void SendEventToPlayersUnreliable(NetworkEvent type, int[] players, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			networkEventManagerClient.ReceiveEvent(type, -1, data);
		}

		public void BroadcastEventToAllPlayersUnreliable(NetworkEvent type, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			networkEventManagerClient.ReceiveEvent(type, -1, data);
		}

		public void BroadcastEventToAllPlayersExceptUnreliable(NetworkEvent type, int playerId, NetworkDependency dependency)
		{
			if (playerId != -1)
			{
				byte[] data = dependency.Serialise();
				networkEventManagerClient.ReceiveEvent(type, -1, data);
			}
		}

		public void BroadcastEventToAllPlayersExperimental(NetworkEvent type, NetworkDependency dependency)
		{
			byte[] data = dependency.Serialise();
			networkEventManagerClient.ReceiveEvent(type, -1, data);
		}

		public void BroadcastEventToAllPlayersExceptExperimental(NetworkEvent type, int playerId, NetworkDependency dependency)
		{
			if (playerId != -1)
			{
				byte[] data = dependency.Serialise();
				networkEventManagerClient.ReceiveEvent(type, -1, data);
			}
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

		public void SetOutputFilter(Predicate<FilterArgs> filter)
		{
		}
	}
}
