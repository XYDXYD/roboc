using Simulation.Hardware.Modules;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class RadarActivationReceivedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private PlayerIdDependency _dependency;

	[Inject]
	public RemoteRadarActivationObservable observable
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (PlayerIdDependency)dependency;
		return this;
	}

	public void Execute()
	{
		int owner = _dependency.owner;
		observable.Dispatch(ref owner);
	}
}
