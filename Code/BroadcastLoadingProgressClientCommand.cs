using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

internal class BroadcastLoadingProgressClientCommand : ICommand
{
	private LoadingProgressDependency _dependency;

	[Inject]
	public INetworkEventManagerClient NetworkEventManagerClient
	{
		private get;
		set;
	}

	public ICommand Inject(LoadingProgressDependency dependency)
	{
		_dependency = dependency;
		return this;
	}

	public void Execute()
	{
		NetworkEventManagerClient.SendEventToServer(NetworkEvent.BroadcastLoadingProgress, _dependency);
	}
}
