using Simulation;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class SetFusionShieldStateClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private FusionShieldStateDependency _dependency;

	[Inject]
	internal FusionShieldsObserver shieldsObserver
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as FusionShieldStateDependency);
		return this;
	}

	public void Execute()
	{
		shieldsObserver.TriggerShieldStateChanged(_dependency.teamId, _dependency.fullPower);
	}
}
