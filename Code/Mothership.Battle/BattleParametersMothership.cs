using Battle;
using LobbyServiceLayer;
using Svelto.IoC;

namespace Mothership.Battle
{
	internal class BattleParametersMothership : BattleParameters, IInitialize
	{
		[Inject]
		internal BattleFoundObserver battleFoundObserver
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			battleFoundObserver.BattleFoundEvent += EnterBattle;
		}

		private void EnterBattle(EnterBattleDependency dependency)
		{
			OnReceivedParameters(dependency.BattleParameters);
		}
	}
}
