using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal class ModuleSelectedObserver : Observer<ItemCategory>
	{
		public ModuleSelectedObserver(ModuleSelectedObservable observable)
			: base(observable)
		{
		}
	}
}
