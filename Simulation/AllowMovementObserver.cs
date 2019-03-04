using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class AllowMovementObserver : Observer<bool>
	{
		public AllowMovementObserver(AllowMovementObservable observable)
			: base(observable)
		{
		}
	}
}
