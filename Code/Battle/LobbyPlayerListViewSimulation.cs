using Svelto.IoC;

namespace Battle
{
	internal sealed class LobbyPlayerListViewSimulation : LobbyPlayerListView
	{
		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		protected override PlayerListElementSimulation AddPlayer(PlayerDataDependency player)
		{
			bool flag = IsMyTeam(player.TeamId);
			bool isPresent = playerNamesContainer.IsNameRegistered(player.PlayerName);
			bool isMegabot = player.Cpu > base.maxRegularCPU;
			string filteredRobotName = FilterProfanity(flag, player.RobotName);
			PlayerListElementSimulation result;
			if (flag)
			{
				bool isPlatoonMate = IsInMyPlatoon(player);
				bool isMe = IsMe(player.PlayerName);
				result = myTeam.AddPlayer(player, filteredRobotName, base.maxCPU, isMegabot);
				myTeam.SetPlayerColour(player.PlayerName, isMe, isAlly: true, isPlatoonMate, isPresent);
			}
			else
			{
				result = enemyTeam.AddPlayer(player, filteredRobotName, base.maxCPU, isMegabot);
				enemyTeam.SetPlayerColour(player.PlayerName, isMe: false, isAlly: false, isPlatoonMate: false, isPresent);
			}
			return result;
		}
	}
}
