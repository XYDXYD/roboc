using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class NetworkPlayerBlinkedObserver : Observer<NetworkPlayerBlinkedData>
	{
		public NetworkPlayerBlinkedObserver(NetworkPlayerBlinkedObservable observable)
			: base(observable)
		{
		}
	}
}
