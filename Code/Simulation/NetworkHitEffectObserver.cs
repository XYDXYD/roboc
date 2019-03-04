using Simulation.Hardware.Weapons;
using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class NetworkHitEffectObserver : Observer<HitInfo>
	{
		public NetworkHitEffectObserver(NetworkHitEffectObservable observable)
			: base(observable)
		{
		}
	}
}
