namespace SocialServiceLayer
{
	internal class ChangeClanMemberRankDependency
	{
		public readonly string UserName;

		public readonly ClanMemberRank ClanMemberRank;

		public ChangeClanMemberRankDependency(string userName, ClanMemberRank clanMemberRank)
		{
			UserName = userName;
			ClanMemberRank = clanMemberRank;
		}
	}
}
