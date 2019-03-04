namespace Mothership.GUI.Social
{
	public class SocialReceivedPartyInviteMessage
	{
		public bool pendingInvite
		{
			get;
			private set;
		}

		public SocialReceivedPartyInviteMessage(bool pendingInvite)
		{
			this.pendingInvite = pendingInvite;
		}
	}
}
