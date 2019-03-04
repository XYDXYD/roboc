using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class RespondToPingRequestClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private RequestPingDependency _dependnecy;

	[Inject]
	public INetworkEventManagerClient networkEventManagerClient
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependnecy = (dependency as RequestPingDependency);
		return this;
	}

	public void Execute()
	{
		networkEventManagerClient.SendEventToServer(NetworkEvent.SetClientPing, _dependnecy);
	}
}
