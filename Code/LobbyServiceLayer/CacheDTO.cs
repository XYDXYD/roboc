using SocialServiceLayer;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;

namespace LobbyServiceLayer
{
	internal static class CacheDTO
	{
		public static string GameHostIP = null;

		public static int? GameHostPort = null;

		public static string MapName = null;

		public static GameModeKey? GameMode = null;

		public static NetworkConfig NetworkConfigs;

		public static string ReconnectGameGUID;

		public static ReadOnlyDictionary<string, PlayerDataDependency> ExpectedPlayers;

		public static int? LocalPlayerTeam = null;

		public static HashSet<int> AllTeams = null;

		public static DateTime ReceiveEnterBattleTime = DateTime.UtcNow;

		public static ReadOnlyDictionary<string, AvatarInfo> AvatarInfo;

		public static ReadOnlyDictionary<string, ClanInfo> ClanInfos
		{
			get;
			set;
		}
	}
}
