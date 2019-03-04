using Svelto.Observer.IntraNamespace;

namespace Simulation
{
	internal sealed class TeslaFireObserver : Observer<int>
	{
		public TeslaFireObserver(TeslaFireObservable teslaFireObservable)
			: base(teslaFireObservable)
		{
		}
	}
}
