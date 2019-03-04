using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class HardwareEnabledObserver : Observer<ItemDescriptor>
	{
		public HardwareEnabledObserver(HardwareEnabledObservable observable)
			: base(observable)
		{
		}
	}
}
