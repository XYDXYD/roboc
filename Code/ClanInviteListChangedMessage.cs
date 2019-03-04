using SocialServiceLayer;

internal class ClanInviteListChangedMessage
{
	public ClanInvite[] ClanInvitations
	{
		get;
		set;
	}

	public ClanInviteListChangedMessage(ClanInvite[] ClanInvitations_)
	{
		ClanInvitations = ClanInvitations_;
	}
}
