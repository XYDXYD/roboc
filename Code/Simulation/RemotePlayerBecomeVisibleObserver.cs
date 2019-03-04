using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class RemotePlayerBecomeVisibleObserver : Observer<int>
	{
		public RemotePlayerBecomeVisibleObserver(RemotePlayerBecomeVisibleObservable observable)
			: base(observable)
		{
		}
	}
}
