using Svelto.ES.Legacy;

namespace Simulation
{
	internal interface IBattleStatsPresenterComponent : IComponent
	{
		void ShowBattleStats();

		void HideBattleStats();

		void ChangeStateToGameEnded();

		void RegisterPlayer(ref RegisterPlayerData registerPlayerData);

		bool CheckIsMyTeam(int teamId);
	}
}
