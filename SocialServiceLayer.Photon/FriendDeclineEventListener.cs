using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class FriendDeclineEventListener : SocialEventListener<FriendListUpdate>, IFriendDeclineEventListener, IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendInviteDeclined;

		protected override void ParseData(EventData eventData)
		{
			string decliner = (string)eventData.Parameters[1];
			string displayName = (string)eventData.Parameters[75];
			Friend friend = CacheDTO.friendList.Find((Friend f) => f.Name == decliner);
			if (friend != null)
			{
				CacheDTO.friendList.Remove(friend);
			}
			FriendListUpdate data = new FriendListUpdate(decliner, displayName, CacheDTO.friendList.AsReadOnly());
			Invoke(data);
		}
	}
}
