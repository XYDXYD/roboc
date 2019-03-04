using Svelto.Observer.IntraNamespace;

namespace Simulation.Hardware.Modules
{
	internal class RemoteRadarActivationObserver : Observer<int>
	{
		public RemoteRadarActivationObserver(RemoteRadarActivationObservable observable)
			: base(observable)
		{
		}
	}
}
