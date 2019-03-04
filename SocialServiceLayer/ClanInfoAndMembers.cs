namespace SocialServiceLayer
{
	internal class ClanInfoAndMembers
	{
		public readonly ClanInfo ClanInfo;

		public readonly ClanMember[] ClanMembers;

		public readonly float XPtoRobitsConversionFactor;

		public ClanInfoAndMembers(ClanInfo clanInfo, ClanMember[] clanMembers, float XPtoRobitsConversionFactor_)
		{
			ClanInfo = clanInfo;
			ClanMembers = clanMembers;
			XPtoRobitsConversionFactor = XPtoRobitsConversionFactor_;
		}
	}
}
