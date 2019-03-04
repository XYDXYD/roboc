using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class WeaponFireEffectOnlyClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private DestroyCubeEffectOnlyDependency _dependency;

	[Inject]
	public INetworkEventManagerClient eventManagerClient
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as DestroyCubeEffectOnlyDependency);
		return this;
	}

	public void Execute()
	{
		eventManagerClient.SendEventToServerExperimental(NetworkEvent.DamageCubeEffectOnly, _dependency);
	}
}
