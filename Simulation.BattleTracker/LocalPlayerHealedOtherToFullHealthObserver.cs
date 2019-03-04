using Svelto.Observer.IntraNamespace;

namespace Simulation.BattleTracker
{
	internal sealed class LocalPlayerHealedOtherToFullHealthObserver : Observer<int>
	{
		public LocalPlayerHealedOtherToFullHealthObserver(LocalPlayerHealedOtherToFullHealthObservable observable)
			: base(observable)
		{
		}
	}
}
