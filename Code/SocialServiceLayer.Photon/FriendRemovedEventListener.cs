using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class FriendRemovedEventListener : SocialEventListener<FriendListUpdate>, IFriendRemovedEventListener, IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendRemoved;

		protected override void ParseData(EventData eventData)
		{
			string remover = (string)eventData.Parameters[1];
			string displayName = (string)eventData.Parameters[75];
			Friend friend = CacheDTO.friendList.Find((Friend f) => f.Name == remover);
			if (friend != null)
			{
				CacheDTO.friendList.Remove(friend);
			}
			FriendListUpdate data = new FriendListUpdate(remover, displayName, CacheDTO.friendList.AsReadOnly());
			Invoke(data);
		}
	}
}
