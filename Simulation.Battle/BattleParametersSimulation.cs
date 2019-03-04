using Battle;
using LobbyServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;

namespace Simulation.Battle
{
	internal class BattleParametersSimulation : BattleParameters, IInitialize
	{
		[Inject]
		internal ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			lobbyRequestFactory.Create<IGetBattleParametersRequest>().SetAnswer(new ServiceAnswer<BattleParametersData>(OnReceivedParameters)).Execute();
		}
	}
}
