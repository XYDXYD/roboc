using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class SetFinalGameScoreClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SetFinalGameScoreDependency _dependency;

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
			_dependency = (dependency as SetFinalGameScoreDependency);
			return this;
		}

		public void Execute()
		{
			battleStatsPresenter.TryUpdateStatValue(_dependency.playerId, InGameStatId.Score, _dependency.score, 0u, isFinal: true);
			InGamePlayerStats inGamePlayerStats = new InGamePlayerStats(_dependency.playerId, new List<InGameStat>());
			inGamePlayerStats.inGameStats.Add(new InGameStat(InGameStatId.Score, 0u, _dependency.score));
			inGameStatsUpdatedObservable.Dispatch(ref inGamePlayerStats);
		}
	}
}
