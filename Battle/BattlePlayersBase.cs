using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using Utility;

namespace Battle
{
	internal abstract class BattlePlayersBase
	{
		protected Dictionary<string, PlayerDataDependency> _expectedPlayers = new Dictionary<string, PlayerDataDependency>(StringComparer.OrdinalIgnoreCase);

		public ICollection<PlayerDataDependency> GetExpectedPlayers()
		{
			Console.Log("BattlePlayers.GetExpectedPlayers()");
			return _expectedPlayers.Values;
		}

		public Dictionary<string, PlayerDataDependency> GetExpectedPlayersDict()
		{
			return _expectedPlayers;
		}

		public List<PlayerDataDependency> GetExpectedPlayersList()
		{
			return new List<PlayerDataDependency>(_expectedPlayers.Values);
		}

		public uint GetTeamId(string userName)
		{
			return (uint)GetPlayerData(userName).TeamId;
		}

		public int GetPlatoonId(string userName)
		{
			return GetPlayerData(userName).PlatoonId;
		}

		public virtual void SetExpectedPlayers(ReadOnlyDictionary<string, PlayerDataDependency> players)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			_expectedPlayers = new Dictionary<string, PlayerDataDependency>((IDictionary<string, PlayerDataDependency>)(object)players);
		}

		protected PlayerDataDependency GetPlayerData(string userName)
		{
			if (_expectedPlayers.TryGetValue(userName, out PlayerDataDependency value))
			{
				return value;
			}
			throw new Exception("No entry found player " + userName);
		}
	}
}
