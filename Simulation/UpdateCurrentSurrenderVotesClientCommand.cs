using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class UpdateCurrentSurrenderVotesClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private CurrentSurrenderVotesDependency _dependency;

		[Inject]
		internal SurrenderControllerClient surrenderControllerClient
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as CurrentSurrenderVotesDependency);
			return this;
		}

		public void Execute()
		{
			surrenderControllerClient.UpdateVotes(_dependency.currentVotes);
		}
	}
}
