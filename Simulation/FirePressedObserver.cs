using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class FirePressedObserver : Observer<int>
	{
		public FirePressedObserver(FirePressedObservable observable)
			: base(observable)
		{
		}
	}
}
