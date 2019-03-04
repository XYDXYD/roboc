using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class WeaponNoFireObserver : Observer<int>
	{
		public WeaponNoFireObserver(WeaponNoFireObservable observable)
			: base(observable)
		{
		}
	}
}
