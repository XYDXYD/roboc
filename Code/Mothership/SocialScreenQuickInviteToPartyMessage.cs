namespace Mothership
{
	internal class SocialScreenQuickInviteToPartyMessage
	{
		public string inviteeName
		{
			get;
			private set;
		}

		public SocialScreenQuickInviteToPartyMessage(string inviteeName)
		{
			this.inviteeName = inviteeName;
		}
	}
}
