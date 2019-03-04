using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class WeaponFiredObserver : Observer<float>
	{
		public WeaponFiredObserver(WeaponFiredObservable observable)
			: base(observable)
		{
		}
	}
}
