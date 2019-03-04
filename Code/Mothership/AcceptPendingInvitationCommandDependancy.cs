namespace Mothership
{
	internal class AcceptPendingInvitationCommandDependancy
	{
		public string ClanName;

		public string InviterName;

		public AcceptPendingInvitationCommandDependancy(string clanName_, string inviterName_)
		{
			ClanName = clanName_;
			InviterName = inviterName_;
		}
	}
}
