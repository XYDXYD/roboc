using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class HardwareDestroyedObserver : Observer<ItemDescriptor>
	{
		public HardwareDestroyedObserver(HardwareDestroyedObservable observable)
			: base(observable)
		{
		}
	}
}
