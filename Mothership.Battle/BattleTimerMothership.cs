using Battle;
using LobbyServiceLayer;
using Svelto.IoC;

namespace Mothership.Battle
{
	internal class BattleTimerMothership : BattleTimer, IInitialize
	{
		[Inject]
		internal BattleFoundObserver battleFoundObserver
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			battleFoundObserver.EnterBattleEvent += OnEnterBattle;
		}

		private void OnEnterBattle(EnterBattleDependency dependency)
		{
			GameInitialised();
		}
	}
}
