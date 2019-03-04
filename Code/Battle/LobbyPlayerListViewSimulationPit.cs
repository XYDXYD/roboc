using Svelto.IoC;

namespace Battle
{
	internal sealed class LobbyPlayerListViewSimulationPit : LobbyPlayerListView
	{
		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		protected override PlayerListElementSimulation AddPlayer(PlayerDataDependency player)
		{
			bool flag = IsMe(player.PlayerName);
			string filteredRobotName = FilterProfanity(flag, player.RobotName);
			bool isMegabot = player.Cpu > base.maxRegularCPU;
			PlayerListElementSimulation result;
			if (flag)
			{
				result = myTeam.AddPlayer(player, filteredRobotName, base.maxCPU, isMegabot);
				myTeam.SetPlayerColour(player.PlayerName, isMe: true, isAlly: true, isPlatoonMate: true, isPresent: true);
			}
			else
			{
				result = enemyTeam.AddPlayer(player, filteredRobotName, base.maxCPU, isMegabot);
				enemyTeam.SetPlayerColour(player.PlayerName, isMe: false, isAlly: false, isPlatoonMate: false, playerNamesContainer.IsNameRegistered(player.PlayerName));
			}
			return result;
		}
	}
}
