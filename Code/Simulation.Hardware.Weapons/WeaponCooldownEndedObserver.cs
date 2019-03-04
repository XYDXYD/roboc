using Svelto.Observer.IntraNamespace;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponCooldownEndedObserver : Observer<ItemDescriptor>
	{
		public WeaponCooldownEndedObserver(WeaponCooldownEndedObservable observable)
			: base(observable)
		{
		}
	}
}
