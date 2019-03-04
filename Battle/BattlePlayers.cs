using Authentication;
using LobbyServiceLayer;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using Utility;

namespace Battle
{
	internal class BattlePlayers : BattlePlayersBase, IInitialize
	{
		public const short NO_PLATOON_ID = 255;

		protected int _myPlatoonId = -1;

		protected uint _myTeam = uint.MaxValue;

		[Inject]
		public ILobbyRequestFactory lobbyRequestFactory
		{
			protected get;
			set;
		}

		public uint MyTeam
		{
			get
			{
				if (_myTeam == uint.MaxValue)
				{
					Console.LogError("BattlePlayers.MyTeam used before being initialized!");
				}
				return _myTeam;
			}
		}

		public int MyPlatoonId
		{
			get
			{
				if (_myPlatoonId == -1)
				{
					Console.LogError("BattlePlayers.MyPlatoonId used before being initialized!");
				}
				return _myPlatoonId;
			}
		}

		public virtual void OnDependenciesInjected()
		{
			lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(((BattlePlayersBase)this).SetExpectedPlayers)).Execute();
		}

		public string GetPlayerRobotName(string userName)
		{
			return GetPlayerData(userName).RobotName;
		}

		public int GetMasteryLevel(string userName)
		{
			return GetPlayerData(userName).MasteryLevel;
		}

		public HashSet<int> GetAllTeams()
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (PlayerDataDependency value in _expectedPlayers.Values)
			{
				if (!hashSet.Contains(value.TeamId))
				{
					hashSet.Add(value.TeamId);
				}
			}
			return hashSet;
		}

		public override void SetExpectedPlayers(ReadOnlyDictionary<string, PlayerDataDependency> players)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			base.SetExpectedPlayers(players);
			PlayerDataDependency playerDataDependency = players.get_Item(User.Username);
			_myTeam = (uint)playerDataDependency.TeamId;
			_myPlatoonId = playerDataDependency.PlatoonId;
		}

		internal bool GetIsInPlatoon(string name)
		{
			return GetPlayerData(name).PlatoonId != 255;
		}
	}
}
