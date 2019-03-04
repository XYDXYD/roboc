using Svelto.Observer.IntraNamespace;

namespace Mothership
{
	internal sealed class GarageChangedObserver : Observer<GarageSlotDependency>
	{
		public GarageChangedObserver(GarageChangedObservable observable)
			: base(observable)
		{
		}
	}
}
