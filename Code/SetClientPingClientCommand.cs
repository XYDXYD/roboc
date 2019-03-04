using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

internal sealed class SetClientPingClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private RequestPingDependency _dependency;

	[Inject]
	public MachineTimeManager machineTimeManager
	{
		get;
		set;
	}

	public void Execute()
	{
		if (machineTimeManager.IsMachineRegistered(_dependency.playerId))
		{
			float ping = (Time.get_time() - _dependency.timeStamp) * 0.5f;
			IMachineUpdater updaterForPlayer = machineTimeManager.GetUpdaterForPlayer(_dependency.playerId);
			updaterForPlayer.SetPing(ping);
		}
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as RequestPingDependency);
		return this;
	}
}
