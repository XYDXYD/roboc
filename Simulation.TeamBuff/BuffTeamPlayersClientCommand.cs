using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.TeamBuff
{
	internal sealed class BuffTeamPlayersClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private TeamBuffDependency _teamBuffDependency;

		[Inject]
		internal BuffTeamObservable buffTeamObservable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_teamBuffDependency = (dependency as TeamBuffDependency);
			return this;
		}

		public void Execute()
		{
			buffTeamObservable.Dispatch(ref _teamBuffDependency);
		}
	}
}
