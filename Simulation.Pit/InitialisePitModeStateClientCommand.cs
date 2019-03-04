using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Pit
{
	internal class InitialisePitModeStateClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private PitModeStateDependency _dependency;

		[Inject]
		internal PitModeHudPresenter pitModeHudPresenter
		{
			get;
			set;
		}

		[Inject]
		internal PitLeaderObserver pitLeaderObserver
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as PitModeStateDependency);
			return this;
		}

		public void Execute()
		{
			if (_dependency.leaderId >= 0)
			{
				pitLeaderObserver.OnBecamePitLeader(_dependency.leaderId);
			}
			pitModeHudPresenter.InitialiseStats(_dependency.playerScores, _dependency.currentStreaks, _dependency.leaderId);
		}
	}
}
