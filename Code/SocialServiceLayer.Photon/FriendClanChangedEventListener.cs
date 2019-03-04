using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class FriendClanChangedEventListener : SocialEventListener<FriendClanChangedEventArgs>, IFriendClanChangedEventListener, IServiceEventListener<FriendClanChangedEventArgs>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendClanChanged;

		protected override void ParseData(EventData eventData)
		{
			string text = (string)eventData.Parameters[1];
			string clanName = (string)eventData.Parameters[31];
			List<Friend> friendList = CacheDTO.friendList;
			for (int i = 0; i < friendList.Count; i++)
			{
				Friend friend = friendList[i];
				if (friend.Name == text)
				{
					friend.ClanName = clanName;
					break;
				}
			}
			Invoke(new FriendClanChangedEventArgs(text, clanName));
		}
	}
}
