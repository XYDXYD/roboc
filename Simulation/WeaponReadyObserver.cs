using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class WeaponReadyObserver : Observer<int>
	{
		public WeaponReadyObserver(WeaponReadyObservable observable)
			: base(observable)
		{
		}
	}
}
