using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class FireHeldDownObserver : Observer<int>
	{
		public FireHeldDownObserver(FireHeldDownObservable observable)
			: base(observable)
		{
		}
	}
}
