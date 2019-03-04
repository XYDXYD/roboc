using Authentication;
using LobbyServiceLayer;
using Svelto.DataStructures;
using System.Collections.Generic;

namespace Battle
{
	internal class BattlePlayersMothership : BattlePlayers
	{
		public override void OnDependenciesInjected()
		{
		}

		public void ClearExpectedPlayersForSoloMatches()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			_expectedPlayers.Clear();
			base.lobbyRequestFactory.Create<ISetExpectedPlayerRequest, ReadOnlyDictionary<string, PlayerDataDependency>>(new ReadOnlyDictionary<string, PlayerDataDependency>(_expectedPlayers)).Execute();
		}

		public void SetExpectedPlayersForSoloMatches(Dictionary<string, PlayerDataDependency> expectedPlayers)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			PlayerDataDependency playerDataDependency = expectedPlayers[User.Username];
			_myTeam = (uint)playerDataDependency.TeamId;
			_expectedPlayers = expectedPlayers;
			_myPlatoonId = 255;
			ReadOnlyDictionary<string, PlayerDataDependency> param = default(ReadOnlyDictionary<string, PlayerDataDependency>);
			param._002Ector(_expectedPlayers);
			base.lobbyRequestFactory.Create<ISetExpectedPlayerRequest, ReadOnlyDictionary<string, PlayerDataDependency>>(param).Execute();
		}

		public void SetExpectedPlayersForMatches(ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			SetExpectedPlayers(expectedPlayers);
		}
	}
}
