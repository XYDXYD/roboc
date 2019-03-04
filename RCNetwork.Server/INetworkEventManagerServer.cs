using RCNetwork.Events;
using Svelto.Command.Dispatcher;
using System;

namespace RCNetwork.Server
{
	internal interface INetworkEventManagerServer
	{
		void RegisterEvent(NetworkEvent type, IDispatchableCommand command);

		void RegisterEvent<T>(NetworkEvent type, IDispatchableCommand command) where T : NetworkDependency;

		void SendEventToPlayer(NetworkEvent type, int player, NetworkDependency dependency);

		void SendEventToPlayers(NetworkEvent type, int[] players, NetworkDependency dependency);

		void BroadcastEventToAllPlayers(NetworkEvent type, NetworkDependency dependency);

		void BroadcastEventToAllPlayersExcept(NetworkEvent type, int ignoredPlayerId, NetworkDependency dependency);

		void SendEventToPlayerUnreliable(NetworkEvent type, int player, NetworkDependency dependency);

		void SendEventToPlayersUnreliable(NetworkEvent type, int[] players, NetworkDependency dependency);

		void BroadcastEventToAllPlayersUnreliable(NetworkEvent type, NetworkDependency dependency);

		void BroadcastEventToAllPlayersExceptUnreliable(NetworkEvent type, int ignoredPlayerId, NetworkDependency dependency);

		void SetOutputFilter(Predicate<FilterArgs> filter);

		void BroadcastEventToAllPlayersExperimental(NetworkEvent type, NetworkDependency dependency);

		void BroadcastEventToAllPlayersExceptExperimental(NetworkEvent type, int ignoredPlayerId, NetworkDependency dependency);

		void ReceiveEvent<T>(NetworkEvent type, T player, byte[] data);
	}
}
