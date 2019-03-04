using RCNetwork.Events;
using Svelto.Command.Dispatcher;

internal interface INetworkEventManagerClient
{
	void RegisterEvent(NetworkEvent type, IDispatchableCommand command);

	void RegisterEvent<T>(NetworkEvent type, IDispatchableCommand command) where T : NetworkDependency;

	void SendEventToServer(NetworkEvent type, NetworkDependency dependency);

	void SendEventToServerUnreliable(NetworkEvent type, NetworkDependency dependency);

	void SendEventToServerExperimental(NetworkEvent type, NetworkDependency dependency);

	void ReceiveEvent<T>(NetworkEvent type, T player, byte[] data);
}
