using Simulation.Sight;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class EnemySpottedReceivedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private PlayerIdDependency _dependency;

	[Inject]
	public RemoteEnemySpottedObservable observable
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as PlayerIdDependency);
		return this;
	}

	public void Execute()
	{
		int owner = _dependency.owner;
		observable.Dispatch(ref owner);
	}
}
