using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class NetworkStunMachineObserver : Observer<NetworkStunnedMachineData>
	{
		public NetworkStunMachineObserver(NetworkStunMachineObservable observable)
			: base(observable)
		{
		}
	}
}
