using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal class GetFriendListResponse
	{
		public readonly IList<Friend> friendsList;

		public GetFriendListResponse(IList<Friend> friendsList_)
		{
			friendsList = friendsList_;
		}
	}
}
