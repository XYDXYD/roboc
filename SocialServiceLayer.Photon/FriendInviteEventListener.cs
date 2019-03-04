using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Utility;

namespace SocialServiceLayer.Photon
{
	internal class FriendInviteEventListener : SocialEventListener<FriendListUpdate>, IFriendInviteEventListener, IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendInviteReceived;

		protected override void ParseData(EventData eventData)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			string text = (string)eventData.Parameters[1];
			string displayName = (string)eventData.Parameters[75];
			string text2 = (string)eventData.Parameters[31];
			int num = -1;
			for (int i = 0; i < CacheDTO.friendList.Count; i++)
			{
				if (CacheDTO.friendList[i].Name == text)
				{
					Console.LogWarning("Inviter already existed - overwriting to match data on server");
					CacheDTO.friendList[i].InviteStatus = FriendInviteStatus.InvitePending;
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				CacheDTO.friendList.Add(new Friend(text, displayName, FriendInviteStatus.InvitePending));
				num = CacheDTO.friendList.Count - 1;
			}
			CacheDTO.friendList[num].IsOnline = (bool)eventData.Parameters[2];
			Hashtable val = eventData.Parameters[9];
			CacheDTO.friendList[num].AvatarInfo = new AvatarInfo((bool)val.get_Item((object)"useCustomAvatar"), (int)val.get_Item((object)"avatarId"));
			if (text2 != null)
			{
				CacheDTO.friendList[num].ClanName = text2;
			}
			FriendListUpdate data = new FriendListUpdate(text, displayName, CacheDTO.friendList.AsReadOnly());
			Invoke(data);
		}
	}
}
