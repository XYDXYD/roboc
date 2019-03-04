using ExitGames.Client.Photon;

namespace SocialServiceLayer
{
	internal class ClanMember : IClanMemberListSortingData
	{
		public readonly string Name;

		public readonly string DisplayName;

		public bool OnlineStatus => IsOnline;

		public bool IsInvited => ClanMemberState == ClanMemberState.Invited;

		public bool IsLeader => ClanMemberRank == ClanMemberRank.Leader;

		public bool IsOfficer => ClanMemberRank == ClanMemberRank.Officer;

		public ClanMemberState ClanMemberState
		{
			get;
			set;
		}

		public AvatarInfo AvatarInfo
		{
			get;
			set;
		}

		public ClanMemberRank ClanMemberRank
		{
			get;
			set;
		}

		public bool IsOnline
		{
			get;
			set;
		}

		public int SeasonXP
		{
			get;
			set;
		}

		public ClanMember(string name, string displayName, ClanMemberState clanMemberState, AvatarInfo avatarInfo, ClanMemberRank clanMemberRank, bool isOnline, int seasonXP)
		{
			Name = name;
			DisplayName = displayName;
			ClanMemberState = clanMemberState;
			AvatarInfo = avatarInfo;
			ClanMemberRank = clanMemberRank;
			IsOnline = isOnline;
			SeasonXP = seasonXP;
		}

		public static ClanMember FromHashtable(Hashtable clanMemberRaw)
		{
			string name = (string)clanMemberRaw.get_Item((object)"userName");
			string displayName = (string)clanMemberRaw.get_Item((object)"displayName");
			ClanMemberState clanMemberState = (ClanMemberState)clanMemberRaw.get_Item((object)"memberState");
			bool flag = (bool)clanMemberRaw.get_Item((object)"useCustomAvatar");
			int avatarId = (!flag) ? ((int)clanMemberRaw.get_Item((object)"avatarId")) : 0;
			ClanMemberRank clanMemberRank = (ClanMemberRank)clanMemberRaw.get_Item((object)"rank");
			bool isOnline = (bool)clanMemberRaw.get_Item((object)"isOnline");
			int seasonXP = (int)clanMemberRaw.get_Item((object)"seasonXP");
			return new ClanMember(name, displayName, clanMemberState, new AvatarInfo(flag, avatarId), clanMemberRank, isOnline, seasonXP);
		}
	}
}
