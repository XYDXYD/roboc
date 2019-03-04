using Svelto.Observer.IntraNamespace;

namespace Simulation.SinglePlayer.Rewards
{
	internal class FinalStatsUpdatedObserver : Observer<StatsUpdatedEvent>
	{
		public FinalStatsUpdatedObserver(FinalStatsUpdatedObservable observable)
			: base(observable)
		{
		}
	}
}
