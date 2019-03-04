using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class FriendAcceptEventListener : SocialEventListener<FriendListUpdate>, IFriendAcceptEventListener, IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendInviteAccepted;

		protected override void ParseData(EventData eventData)
		{
			string accepter = (string)eventData.Parameters[1];
			string displayName = (string)eventData.Parameters[75];
			Friend friend = CacheDTO.friendList.Find((Friend f) => f.Name == accepter);
			if (friend != null)
			{
				friend.InviteStatus = FriendInviteStatus.Accepted;
				friend.IsOnline = true;
			}
			FriendListUpdate data = new FriendListUpdate(accepter, displayName, CacheDTO.friendList.AsReadOnly());
			Invoke(data);
		}
	}
}
