using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class OnTeamBaseCaptureResetClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private TeamBaseStateDependency _dependency;

	[Inject]
	public TeamBaseProgressDispatcher teamBaseProgressDispatcher
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as TeamBaseStateDependency);
		return this;
	}

	public void Execute()
	{
		teamBaseProgressDispatcher.OnCaptureReset(_dependency.team);
	}
}
