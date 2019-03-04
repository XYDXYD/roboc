using System.Collections.Generic;

internal class FriendListChangedMessage
{
	public IList<Friend> FriendList
	{
		get;
		set;
	}

	public FriendListChangedMessage(IList<Friend> friendList_)
	{
		FriendList = friendList_;
	}
}
