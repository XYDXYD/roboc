using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class BroadcastGetClientPingsServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private RequestPingDependency _dependency;

	[Inject]
	public INetworkEventManagerServer eventManagerServer
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as RequestPingDependency);
		_dependency.requester = _dependency.senderId;
		return this;
	}

	public void Execute()
	{
		eventManagerServer.SendEventToPlayer(NetworkEvent.GetClientPings, _dependency.playerId, _dependency);
	}
}
