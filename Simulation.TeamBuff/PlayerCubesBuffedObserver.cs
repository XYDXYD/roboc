using Svelto.Observer.IntraNamespace;

namespace Simulation.TeamBuff
{
	internal class PlayerCubesBuffedObserver : Observer<PlayerCubesBuffedDependency>
	{
		public PlayerCubesBuffedObserver(PlayerCubesBuffedObservable observable)
			: base(observable)
		{
		}
	}
}
