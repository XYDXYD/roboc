using Svelto.Observer.IntraNamespace;

namespace Simulation.BattleArena.CapturePoint
{
	internal class CapturePointNotificationObserver : Observer<CapturePointNotificationDependency>
	{
		public CapturePointNotificationObserver(CapturePointNotificationObservable observable)
			: base(observable)
		{
		}
	}
}
