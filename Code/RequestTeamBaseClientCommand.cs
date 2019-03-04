using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class RequestTeamBaseClientCommand : IDispatchableCommand, ICommand
{
	[Inject]
	public INetworkEventManagerClient networkEventManagerClient
	{
		private get;
		set;
	}

	public void Execute()
	{
		networkEventManagerClient.SendEventToServer(NetworkEvent.RequestTeamBaseModel, new NetworkDependency());
	}
}
