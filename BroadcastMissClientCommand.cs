using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

internal sealed class BroadcastMissClientCommand : IInjectableCommand<FireMissDependency>, ICommand
{
	private FireMissDependency _dependency;

	[Inject]
	public INetworkEventManagerClient eventManagerClient
	{
		private get;
		set;
	}

	public ICommand Inject(FireMissDependency dependency)
	{
		_dependency = dependency;
		return this;
	}

	public void Execute()
	{
		eventManagerClient.SendEventToServerUnreliable(NetworkEvent.FireMiss, _dependency);
	}
}
