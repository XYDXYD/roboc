using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class UpdateTeamBaseProgressClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
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
		teamBaseProgressDispatcher.OnBaseProgressChanged(_dependency.team, _dependency.currentProgress);
	}
}
