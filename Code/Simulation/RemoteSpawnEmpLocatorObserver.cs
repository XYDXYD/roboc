using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class RemoteSpawnEmpLocatorObserver : Observer<RemoteLocatorData>
	{
		public RemoteSpawnEmpLocatorObserver(RemoteSpawnEmpLocatorObservable observable)
			: base(observable)
		{
		}
	}
}
