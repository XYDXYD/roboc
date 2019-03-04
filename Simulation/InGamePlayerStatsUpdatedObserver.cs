using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class InGamePlayerStatsUpdatedObserver : Observer<InGamePlayerStats>
	{
		public InGamePlayerStatsUpdatedObserver(InGamePlayerStatsUpdatedObservable observable)
			: base(observable)
		{
		}
	}
}
