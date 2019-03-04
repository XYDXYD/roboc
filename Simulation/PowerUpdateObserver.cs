using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class PowerUpdateObserver : Observer<int>
	{
		public PowerUpdateObserver(PowerUpdateObservable observable)
			: base(observable)
		{
		}
	}
}
