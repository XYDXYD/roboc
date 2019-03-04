using Svelto.Observer.IntraNamespace;

namespace Simulation.BattleTracker
{
	internal sealed class LocalPlayerMadeKillObserver : Observer<PlayerKillData>
	{
		public LocalPlayerMadeKillObserver(LocalPlayerMadeKillObservable observable)
			: base(observable)
		{
		}
	}
}
