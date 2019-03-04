using RCNetwork.Events;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class WeaponFireEffectClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private WeaponFireEffectDependency _dependency;

	[Inject]
	public INetworkEventManagerClient eventManagerClient
	{
		private get;
		set;
	}

	[Inject]
	public NetworkMachineManager machineManager
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as WeaponFireEffectDependency);
		return this;
	}

	public void Execute()
	{
		eventManagerClient.SendEventToServerExperimental(NetworkEvent.FireWeaponEffect, _dependency);
	}
}
