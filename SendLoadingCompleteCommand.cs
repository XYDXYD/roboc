using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

internal class SendLoadingCompleteCommand : ICommand
{
	[Inject]
	public INetworkEventManagerClient NetworkEventManagerClient
	{
		private get;
		set;
	}

	public void Execute()
	{
		NetworkEventManagerClient.SendEventToServer(NetworkEvent.LoadingComplete, new NetworkDependency());
	}
}
