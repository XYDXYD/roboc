using Svelto.Observer.IntraNamespace;

namespace Simulation.Hardware.Modules
{
	internal class TeamRadarObserver : Observer<bool>
	{
		public TeamRadarObserver(TeamRadarObservable observable)
			: base(observable)
		{
		}
	}
}
