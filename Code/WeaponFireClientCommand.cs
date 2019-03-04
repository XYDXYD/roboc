using RCNetwork.Events;
using Simulation;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class WeaponFireClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private DestroyCubeDependency _dependency;

	[Inject]
	public INetworkEventManagerClient eventManagerClient
	{
		private get;
		set;
	}

	[Inject]
	public WeaponFireStateSync fireStateSync
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as DestroyCubeDependency);
		return this;
	}

	public void Execute()
	{
		fireStateSync.DamageReportedToServer(_dependency, _dependency.targetType);
		eventManagerClient.SendEventToServer(NetworkEvent.DamageCube, _dependency);
	}
}
