using Svelto.Observer.IntraNamespace;

namespace Simulation.BattleArena.Equalizer
{
	internal class EqualizerNotificationObserver : Observer<EqualizerNotificationDependency>
	{
		public EqualizerNotificationObserver(EqualizerNotificationObservable observable)
			: base(observable)
		{
		}
	}
}
