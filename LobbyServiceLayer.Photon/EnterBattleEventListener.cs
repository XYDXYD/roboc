using Authentication;
using Battle;
using ExitGames.Client.Photon;
using SocialServiceLayer;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace LobbyServiceLayer.Photon
{
	internal class EnterBattleEventListener : LobbyEventListener<EnterBattleDependency>, IEnterBattleEventListener, IServiceEventListener<EnterBattleDependency>, IServiceEventListenerBase
	{
		public override LobbyEventCode LobbyEventCode => LobbyEventCode.EnterBattle;

		protected override void ParseData(EventData eventData)
		{
			EnterBattleDependency enterBattleDependency = ParseData(eventData.Parameters, DateTime.UtcNow);
			CacheData(enterBattleDependency);
			Invoke(enterBattleDependency);
		}

		public static void CacheData(EnterBattleDependency dependency)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			HashSet<int> hashSet = new HashSet<int>();
			int value = -1;
			DictionaryEnumerator<string, PlayerDataDependency> enumerator = dependency.Players.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, PlayerDataDependency> current = enumerator.get_Current();
					string key = current.Key;
					PlayerDataDependency value2 = current.Value;
					hashSet.Add(value2.TeamId);
					if (key.ToUpper() == User.Username.ToUpper())
					{
						value = value2.TeamId;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			CacheDTO.LocalPlayerTeam = value;
			CacheDTO.ReceiveEnterBattleTime = dependency.BattleParameters.ReceivedEnterBattleTime;
			CacheDTO.NetworkConfigs = dependency.BattleParameters.NetworkConfigs;
			CacheDTO.AllTeams = hashSet;
			CacheDTO.GameHostIP = dependency.BattleParameters.HostIP;
			CacheDTO.GameHostPort = dependency.BattleParameters.HostPort;
			CacheDTO.MapName = dependency.BattleParameters.MapName;
			CacheDTO.GameMode = dependency.BattleParameters.GameModeKey;
			CacheDTO.ExpectedPlayers = dependency.Players;
			CacheDTO.AvatarInfo = dependency.AvatarInfos;
			CacheDTO.ClanInfos = dependency.ClanInfos;
			CacheDTO.ReconnectGameGUID = dependency.BattleParameters.GameGuid;
		}

		public static EnterBattleDependency ParseData(Dictionary<byte, object> parameters, DateTime receivedEnterBattleTime)
		{
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Expected O, but got Unknown
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			string hostIP = (string)parameters[6];
			int hostPort = (int)parameters[7];
			string mapName = (string)parameters[8];
			GameModeType type = (GameModeType)parameters[1];
			string gameGuid = (string)parameters[40];
			bool isRanked = (bool)parameters[25];
			bool isBrawl = (bool)parameters[26];
			bool isCustomGame = (bool)parameters[27];
			Hashtable[] array = (Hashtable[])parameters[5];
			Dictionary<string, PlayerDataDependency> dictionary = new Dictionary<string, PlayerDataDependency>(array.Length);
			Dictionary<string, AvatarInfo> dictionary2 = new Dictionary<string, AvatarInfo>();
			Dictionary<string, ClanInfo> dictionary3 = new Dictionary<string, ClanInfo>();
			Dictionary<string, List<string>> dictionary4 = new Dictionary<string, List<string>>();
			for (int i = 0; i < array.Length; i++)
			{
				Hashtable val = array[i];
				string text = val.get_Item((object)"name").ToString();
				string empty = string.Empty;
				if (!((Dictionary<object, object>)val).ContainsKey((object)"displayName"))
				{
					throw new Exception("expected display name for player:" + i + "name:" + text);
				}
				empty = ((val.get_Item((object)"displayName") == null) ? val.get_Item((object)"name").ToString() : val.get_Item((object)"displayName").ToString());
				string robotName = val.get_Item((object)"robotName").ToString();
				byte[] cubeMap = (byte[])val.get_Item((object)"cubeMap");
				byte[] colourMap = (byte[])val.get_Item((object)"colourMap");
				string spawnEffect = val.get_Item((object)"spawnEffect").ToString();
				string deathEffect = val.get_Item((object)"deathEffect").ToString();
				string text2 = val.get_Item((object)"groupId").ToString();
				int team = Convert.ToInt32(val.get_Item((object)"team"));
				bool hasPremium = Convert.ToBoolean(val.get_Item((object)"hasPremium"));
				int[] weaponOrder = (int[])val.get_Item((object)"weaponOrder");
				string robotUniqueId = val.get_Item((object)"robotUniqueID").ToString();
				int cpu = Convert.ToInt32(val.get_Item((object)"cpu"));
				bool flag = (bool)val.get_Item((object)"useCustomAvatar");
				int avatarId = (!flag) ? ((int)val.get_Item((object)"avatarId")) : 0;
				int masteryLevel = (int)val.get_Item((object)"masteryLevel");
				int tier = (int)val.get_Item((object)"tier");
				bool flag2 = (bool)val.get_Item((object)"isAI");
				AvatarInfo avatarInfo;
				if (flag2)
				{
					avatarInfo = new AvatarInfo(useCustomAvatar: false, AvatarUtils.ChooseAvatarIdForAi(text));
				}
				else
				{
					avatarInfo = new AvatarInfo(flag, avatarId);
					dictionary2.Add(text, avatarInfo);
				}
				string clanName = null;
				bool useCustomAvatar = false;
				int avatarId2 = 0;
				if (((Dictionary<object, object>)val).ContainsKey((object)"clanName"))
				{
					clanName = (string)val.get_Item((object)"clanName");
					useCustomAvatar = (bool)val.get_Item((object)"clanUseCustomAvatar");
					avatarId2 = (int)val.get_Item((object)"clanDefaultAvatarID");
					if (!dictionary3.ContainsKey(text))
					{
						dictionary3.Add(text, new ClanInfo(clanName, string.Empty, ClanType.Closed));
					}
				}
				if (!string.IsNullOrEmpty(text2) && string.Compare(text, text2, StringComparison.OrdinalIgnoreCase) != 0)
				{
					if (!dictionary4.ContainsKey(text2))
					{
						dictionary4.Add(text2, new List<string>());
					}
					dictionary4[text2].Add(text);
				}
				dictionary.Add(text, new PlayerDataDependency(text, empty, robotName, cubeMap, (uint)team, hasPremium, robotUniqueId, cpu, masteryLevel, tier, avatarInfo, clanName, new AvatarInfo(useCustomAvatar, avatarId2), flag2, spawnEffect, deathEffect, weaponOrder, colourMap));
			}
			SortedList<string, List<string>> sortedList = new SortedList<string, List<string>>();
			foreach (KeyValuePair<string, List<string>> item in dictionary4)
			{
				List<string> value = item.Value;
				value.Sort();
				sortedList.Add(value[0], value);
			}
			int num = 0;
			foreach (KeyValuePair<string, List<string>> item2 in sortedList)
			{
				List<string> value2 = item2.Value;
				foreach (string item3 in value2)
				{
					dictionary[item3].PlatoonId = num;
				}
				num++;
			}
			Hashtable config = parameters[23];
			NetworkConfig networkConfig = new NetworkConfig(config);
			GameModeKey key = new GameModeKey(type, isRanked, isBrawl, isCustomGame);
			BattleParametersData battleParameters = new BattleParametersData(hostIP, hostPort, mapName, key, receivedEnterBattleTime, networkConfig, null, gameGuid);
			return new EnterBattleDependency(new ReadOnlyDictionary<string, PlayerDataDependency>(dictionary), battleParameters, new ReadOnlyDictionary<string, AvatarInfo>(dictionary2), new ReadOnlyDictionary<string, ClanInfo>(dictionary3));
		}
	}
}
