using Simulation.Analytics;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Network
{
	internal sealed class UpdateTeamDeathMatchCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private UpdateTeamDeathMatchDependency _dependency;

		[Inject]
		public TeamDeathMatchStatsPresenter statsPresenter
		{
			private get;
			set;
		}

		[Inject]
		public WorldSwitchingSimulationAnalytics worldSwitchingAnalytics
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as UpdateTeamDeathMatchDependency);
			return this;
		}

		public void Execute()
		{
			statsPresenter.UpdateScore(_dependency.teamScores, _dependency.hasTimeExpired);
			worldSwitchingAnalytics.UpdateTDMTeamKills(_dependency.teamScores);
		}
	}
}
