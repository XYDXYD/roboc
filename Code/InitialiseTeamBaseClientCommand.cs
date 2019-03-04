using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class InitialiseTeamBaseClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
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
		int num = (int)_dependency.currentProgress;
		for (int i = 1; i <= num; i++)
		{
			if (i == num)
			{
				teamBaseProgressDispatcher.OnFinalSectionComplete(_dependency.team);
			}
			else
			{
				teamBaseProgressDispatcher.OnSectionComplete(_dependency.team);
			}
		}
	}
}
