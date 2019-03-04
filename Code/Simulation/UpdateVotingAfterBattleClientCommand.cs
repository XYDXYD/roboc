using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class UpdateVotingAfterBattleClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private UpdateVotingAfterBattleDependency _dependency;

		[Inject]
		public UpdateVotingAfterBattleClientCommandObservable updateVotingAfterBattleClientCommandObservable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as UpdateVotingAfterBattleDependency);
			return this;
		}

		public void Execute()
		{
			updateVotingAfterBattleClientCommandObservable.Dispatch(ref _dependency);
		}
	}
}
