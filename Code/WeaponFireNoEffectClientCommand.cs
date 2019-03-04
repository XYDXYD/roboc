using RCNetwork.Events;
using Simulation;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class WeaponFireNoEffectClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private DestroyCubeNoEffectDependency _dependency;

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

	[Inject]
	public LivePlayersContainer livePlayersContainer
	{
		private get;
		set;
	}

	[Inject]
	public PlayerMachinesContainer playerMachinesContainer
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as DestroyCubeNoEffectDependency);
		return this;
	}

	public void Execute()
	{
		int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(_dependency.targetType, _dependency.hitMachineId);
		fireStateSync.DamageReportedToServer(_dependency, _dependency.targetType);
		eventManagerClient.SendEventToServer(NetworkEvent.DamageCubeNoEffect, _dependency);
	}
}
