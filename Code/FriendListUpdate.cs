using System.Collections.Generic;

internal struct FriendListUpdate
{
	public readonly IList<Friend> friendList;

	public readonly string user;

	public readonly string displayName;

	public FriendListUpdate(string user, string displayName, IList<Friend> friendList)
	{
		this.user = user;
		this.displayName = displayName;
		this.friendList = friendList;
	}
}
