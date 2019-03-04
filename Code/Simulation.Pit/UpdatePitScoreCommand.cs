using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Pit
{
	internal sealed class UpdatePitScoreCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private UpdatePitScoreDependency _dependency;

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

		[Inject]
		internal PitModeMusicManager musicManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as UpdatePitScoreDependency);
			return this;
		}

		public void Execute()
		{
			pitLeaderObserver.OnBecamePitLeader(_dependency.leaderId);
			pitModeHudPresenter.UpdateDisplay(_dependency.playerId, _dependency.score, _dependency.streak, _dependency.destroyedId, _dependency.leaderId);
			musicManager.UpdateScores((int)_dependency.score);
		}
	}
}
