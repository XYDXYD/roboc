using RCNetwork.Events;
using RCNetwork.Server;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class BroadcastSetClientPingServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
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
		return this;
	}

	public void Execute()
	{
		eventManagerServer.SendEventToPlayer(NetworkEvent.SetClientPing, _dependency.requester, _dependency);
	}
}
