namespace SocialServiceLayer
{
	public interface IClanMemberListSortingData
	{
		bool OnlineStatus
		{
			get;
		}

		bool IsInvited
		{
			get;
		}

		bool IsLeader
		{
			get;
		}

		bool IsOfficer
		{
			get;
		}

		int SeasonXP
		{
			get;
		}
	}
}
