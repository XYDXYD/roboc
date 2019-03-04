using SocialServiceLayer;
using System.Collections.Generic;

namespace Mothership
{
	internal class PlayerCategorySorter : IComparer<IClanMemberListSortingData>
	{
		public int Compare(IClanMemberListSortingData memberA, IClanMemberListSortingData memberB)
		{
			bool onlineStatus = memberA.OnlineStatus;
			bool onlineStatus2 = memberB.OnlineStatus;
			bool isInvited = memberA.IsInvited;
			bool isInvited2 = memberB.IsInvited;
			int seasonXP = memberA.SeasonXP;
			int seasonXP2 = memberB.SeasonXP;
			int num = (!isInvited) ? (500 + ((!onlineStatus) ? (-100) : 100) + ((seasonXP > seasonXP2) ? 1 : (-1))) : (-500);
			int num2 = (!isInvited2) ? (500 + ((!onlineStatus2) ? (-100) : 100) + ((seasonXP2 > seasonXP) ? 1 : (-1))) : (-500);
			return num2 - num;
		}
	}
}
