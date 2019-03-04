using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

internal class RequestLoadingProgressAllUsersCommand : ICommand
{
	[Inject]
	public INetworkEventManagerClient NetworkEventManagerClient
	{
		private get;
		set;
	}

	public void Execute()
	{
		NetworkEventManagerClient.SendEventToServer(NetworkEvent.RequestLoadingProgressAllUsers, new NetworkDependency());
	}
}
