using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class FriendCancelledEventListener : SocialEventListener<FriendListUpdate>, IFriendCancelledEventListener, IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendInviteCancelled;

		protected override void ParseData(EventData eventData)
		{
			string canceller = (string)eventData.Parameters[1];
			string displayName = (string)eventData.Parameters[75];
			Friend friend = CacheDTO.friendList.Find((Friend f) => f.Name == canceller);
			if (friend != null)
			{
				CacheDTO.friendList.Remove(friend);
			}
			FriendListUpdate data = new FriendListUpdate(canceller, displayName, CacheDTO.friendList.AsReadOnly());
			Invoke(data);
		}
	}
}
