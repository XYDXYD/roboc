using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class MachineColliderCollectionObserver : Observer<MachineColliderCollectionData>
	{
		public MachineColliderCollectionObserver(MachineColliderCollectionObservable observable)
			: base(observable)
		{
		}
	}
}
