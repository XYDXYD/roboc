using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class LockOnStateObserver : Observer<LockOnData>
	{
		public LockOnStateObserver(LockOnStateObservable observable)
			: base(observable)
		{
		}
	}
}
