using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class InGameStatsInitialiseClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private InitialiseGameStatsDependency _dependency;

		[Inject]
		public BattleStatsPresenter battleStatsPresenter
		{
			get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as InitialiseGameStatsDependency);
			return this;
		}

		public void Execute()
		{
			for (int i = 0; i < _dependency.inGamePlayerStatsList.Count; i++)
			{
				InGamePlayerStats inGamePlayerStats = _dependency.inGamePlayerStatsList[i];
				for (int j = 0; j < inGamePlayerStats.inGameStats.Count; j++)
				{
					InGameStat inGameStat = inGamePlayerStats.inGameStats[j];
					battleStatsPresenter.TryUpdateStatValue(inGamePlayerStats.playerId, inGameStat.ID, inGameStat.Amount, inGameStat.Score);
				}
			}
		}
	}
}
