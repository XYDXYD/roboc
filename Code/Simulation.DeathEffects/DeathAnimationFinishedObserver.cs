using Svelto.Observer;
using Svelto.Observer.IntraNamespace;

namespace Simulation.DeathEffects
{
	internal class DeathAnimationFinishedObserver : Observer<Kill>
	{
		public DeathAnimationFinishedObserver(Observable<Kill> observable)
			: base(observable)
		{
		}
	}
}
