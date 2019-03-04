using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class RemotePlayerBecomeInvisibleObserver : Observer<int>
	{
		public RemotePlayerBecomeInvisibleObserver(RemotePlayerBecomeInvisibleObservable observable)
			: base(observable)
		{
		}
	}
}
