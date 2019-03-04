namespace SocialServiceLayer
{
	internal class ClanMemberXPChangedEventContent
	{
		public readonly string memberName;

		public readonly int newXPValue;

		public ClanMemberXPChangedEventContent(string memberName_, int newXP_)
		{
			memberName = memberName_;
			newXPValue = newXP_;
		}
	}
}
