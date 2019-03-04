using Svelto.Observer.IntraNamespace;

namespace Simulation.BattleArena.CapturePoint
{
	internal class CapturePointProgressObserver : Observer<TeamBaseStateDependency>
	{
		public CapturePointProgressObserver(CapturePointProgressObservable observable)
			: base(observable)
		{
		}
	}
}
