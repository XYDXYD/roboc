using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class UpdateGameStatsClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private UpdateGameStatsDependency _dependency;

		[Inject]
		public BattleStatsPresenter battleStatsPresenter
		{
			get;
			set;
		}

		[Inject]
		public InGamePlayerStatsUpdatedObservable inGameStatsUpdatedObservable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as UpdateGameStatsDependency);
			return this;
		}

		public void Execute()
		{
			battleStatsPresenter.TryUpdateStatValue(_dependency.playerId, _dependency.gameStatsId, _dependency.amount, _dependency.deltaScore);
			battleStatsPresenter.TryUpdateStatValue(_dependency.playerId, InGameStatId.Score, _dependency.score, _dependency.deltaScore);
			InGamePlayerStats inGamePlayerStats = new InGamePlayerStats(_dependency.playerId, new List<InGameStat>());
			inGamePlayerStats.inGameStats.Add(new InGameStat(_dependency.gameStatsId, 0u, _dependency.amount));
			inGamePlayerStats.inGameStats.Add(new InGameStat(InGameStatId.Score, 0u, _dependency.score));
			inGameStatsUpdatedObservable.Dispatch(ref inGamePlayerStats);
		}
	}
}
