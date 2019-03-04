using Svelto.Observer.IntraNamespace;

namespace GameServer
{
	internal class PlayerCloakObserver : Observer<PlayerClockState>
	{
		public PlayerCloakObserver(PlayerCloakObservable observable)
			: base(observable)
		{
		}
	}
}
