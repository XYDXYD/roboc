using Svelto.Observer.IntraNamespace;

namespace Simulation.SinglePlayer.Rewards
{
	internal class StatsUpdatedObserver : Observer<StatsUpdatedEvent>
	{
		public StatsUpdatedObserver(StatsUpdatedObservable observable)
			: base(observable)
		{
		}
	}
}
