namespace SocialServiceLayer
{
	internal class ClanMemberDataChangedEventContent
	{
		public readonly string UserName;

		public readonly string displayName;

		public readonly bool? IsOnline;

		public readonly ClanMemberRank? ClanMemberRank;

		public ClanMemberDataChangedEventContent(string userName, string displayName, bool? isOnline, ClanMemberRank? clanMemberRank)
		{
			UserName = userName;
			UserName = displayName;
			IsOnline = isOnline;
			ClanMemberRank = clanMemberRank;
		}
	}
}
