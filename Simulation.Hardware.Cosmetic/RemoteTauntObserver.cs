using Svelto.Observer;
using Svelto.Observer.IntraNamespace;

namespace Simulation.Hardware.Cosmetic
{
	internal class RemoteTauntObserver : Observer<TauntDependency>
	{
		public RemoteTauntObserver(Observable<TauntDependency> observable)
			: base(observable)
		{
		}
	}
}
