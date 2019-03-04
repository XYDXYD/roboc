namespace Mothership
{
	internal class DeclinePendingInvitationCommandDependancy
	{
		public string ClanName;

		public string InviterName;

		public DeclinePendingInvitationCommandDependancy(string clanName_, string inviterName_)
		{
			ClanName = clanName_;
			InviterName = inviterName_;
		}
	}
}
