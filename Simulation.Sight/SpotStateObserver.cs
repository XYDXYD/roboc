using Svelto.Observer.IntraNamespace;

namespace Simulation.Sight
{
	internal class SpotStateObserver : Observer<SpotStateChangeArgs>
	{
		public SpotStateObserver(SpotStateObservable observable)
			: base(observable)
		{
		}
	}
}
