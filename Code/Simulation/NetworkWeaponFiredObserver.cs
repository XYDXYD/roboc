using Simulation.Hardware.Weapons;
using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class NetworkWeaponFiredObserver : Observer<FiringInfo>
	{
		public NetworkWeaponFiredObserver(NetworkWeaponFiredObservable observable)
			: base(observable)
		{
		}
	}
}
