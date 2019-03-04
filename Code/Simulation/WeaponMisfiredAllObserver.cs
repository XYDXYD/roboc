using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class WeaponMisfiredAllObserver : Observer<int>
	{
		public WeaponMisfiredAllObserver(WeaponMisfiredAllObservable observable)
			: base(observable)
		{
		}
	}
}
