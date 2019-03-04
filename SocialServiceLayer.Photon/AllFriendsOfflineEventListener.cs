using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class AllFriendsOfflineEventListener : SocialEventListener<IList<Friend>>, IAllFriendsOfflineEventListener, IServiceEventListener<IList<Friend>>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.AllFriendsOffline;

		protected override void ParseData(EventData eventData)
		{
			if (CacheDTO.friendList != null)
			{
				foreach (Friend friend in CacheDTO.friendList)
				{
					friend.IsOnline = false;
				}
				Invoke(CacheDTO.friendList.AsReadOnly());
			}
		}
	}
}
