using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class MachineColliderIgnoreObserver : Observer<int>
	{
		public MachineColliderIgnoreObserver(MachineColliderIgnoreObservable observable)
			: base(observable)
		{
		}
	}
}
