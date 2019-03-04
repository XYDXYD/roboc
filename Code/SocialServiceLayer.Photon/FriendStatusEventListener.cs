using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class FriendStatusEventListener : SocialEventListener<Friend, IList<Friend>>, IFriendStatusEventListener, IServiceEventListener<Friend, IList<Friend>>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendStatusUpdate;

		protected override void ParseData(EventData eventData)
		{
			string friendName = (string)eventData.Parameters[1];
			string displayName = (string)eventData.Parameters[75];
			bool isOnline = (bool)eventData.Parameters[2];
			FriendInviteStatus friendInviteStatus = (FriendInviteStatus)eventData.Parameters[3];
			Friend friend;
			if (CacheDTO.friendList != null)
			{
				friend = CacheDTO.friendList.Find((Friend f) => f.Name == friendName);
				if (friend != null)
				{
					friend.InviteStatus = friendInviteStatus;
					friend.IsOnline = isOnline;
				}
				else
				{
					friend = new Friend(friendName, displayName, friendInviteStatus);
					friend.IsOnline = isOnline;
					CacheDTO.friendList.Add(friend);
				}
			}
			else
			{
				friend = new Friend(friendName, displayName, friendInviteStatus);
				friend.IsOnline = isOnline;
				List<Friend> list = new List<Friend>();
				list.Add(friend);
				CacheDTO.friendList = list;
			}
			Invoke(friend, CacheDTO.friendList.AsReadOnly());
		}
	}
}
