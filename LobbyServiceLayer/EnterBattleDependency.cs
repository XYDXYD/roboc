using Battle;
using SocialServiceLayer;
using Svelto.DataStructures;

namespace LobbyServiceLayer
{
	internal class EnterBattleDependency
	{
		public readonly ReadOnlyDictionary<string, PlayerDataDependency> Players;

		public readonly BattleParametersData BattleParameters;

		public readonly ReadOnlyDictionary<string, AvatarInfo> AvatarInfos;

		public readonly ReadOnlyDictionary<string, ClanInfo> ClanInfos;

		public EnterBattleDependency(ReadOnlyDictionary<string, PlayerDataDependency> players, BattleParametersData battleParameters, ReadOnlyDictionary<string, AvatarInfo> avatarInfos, ReadOnlyDictionary<string, ClanInfo> clanInfos)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Players = players;
			BattleParameters = battleParameters;
			AvatarInfos = avatarInfos;
			ClanInfos = clanInfos;
		}
	}
}
