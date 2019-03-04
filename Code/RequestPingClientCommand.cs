using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class RequestPingClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private RequestPingDependency _dependency;

	[Inject]
	public INetworkEventManagerClient networkEventmanagerClient
	{
		private get;
		set;
	}

	public void Execute()
	{
		networkEventmanagerClient.SendEventToServer(NetworkEvent.GetClientPings, _dependency);
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as RequestPingDependency);
		return this;
	}
}
