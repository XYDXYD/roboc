using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class StopSyncingClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private PlayerIdDependency _dependency;

	[Inject]
	public INetworkEventManagerClient eventManagerClient
	{
		private get;
		set;
	}

	[Inject]
	public PlayerTeamsContainer playerTeamsContainer
	{
		private get;
		set;
	}

	[Inject]
	public MachineTimeManager machineTimeManager
	{
		get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as PlayerIdDependency);
		return this;
	}

	public void Execute()
	{
		if (!playerTeamsContainer.IsMe(TargetType.Player, _dependency.owner))
		{
			machineTimeManager.UnregisterUpdater(_dependency.owner);
		}
	}
}
