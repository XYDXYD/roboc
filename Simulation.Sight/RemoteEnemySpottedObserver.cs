using Svelto.Observer.IntraNamespace;

namespace Simulation.Sight
{
	internal class RemoteEnemySpottedObserver : Observer<int>
	{
		public RemoteEnemySpottedObserver(RemoteEnemySpottedObservable observable)
			: base(observable)
		{
		}
	}
}
