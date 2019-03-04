using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class HealingAppliedObserver : Observer<HealingAppliedData>
	{
		public HealingAppliedObserver(HealingAppliedObservable observable)
			: base(observable)
		{
		}
	}
}
