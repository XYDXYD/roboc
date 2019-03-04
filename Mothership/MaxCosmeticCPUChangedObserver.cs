using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal class MaxCosmeticCPUChangedObserver : Observer<uint>
	{
		public MaxCosmeticCPUChangedObserver(MaxCosmeticCPUChangedObservable observable)
			: base(observable)
		{
		}
	}
}
