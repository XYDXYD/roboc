using Simulation;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class MapPingEventClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private MapPingEventDependency _mapPingDependency;

	[Inject]
	private MapPingClientCommandObserver clientCommandObserver
	{
		get;
		set;
	}

	[Inject]
	private PlayerNamesContainer playerNamesContainer
	{
		get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_mapPingDependency = (dependency as MapPingEventDependency);
		return this;
	}

	public void Execute()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		clientCommandObserver.SpawnPing(_mapPingDependency.location, _mapPingDependency.type, _mapPingDependency.sender);
	}
}
