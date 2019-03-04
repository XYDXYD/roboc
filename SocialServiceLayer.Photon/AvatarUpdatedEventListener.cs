using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class AvatarUpdatedEventListener : SocialEventListener<AvatarUpdatedUpdate>, IAvatarUpdatedEventListener, IServiceEventListener<AvatarUpdatedUpdate>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.AvatarUpdated;

		protected override void ParseData(EventData eventData)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			string text = (string)eventData.Parameters[1];
			Hashtable val = eventData.Parameters[9];
			AvatarInfo avatarInfo = new AvatarInfo((bool)val.get_Item((object)"useCustomAvatar"), (int)val.get_Item((object)"avatarId"));
			for (int i = 0; i < CacheDTO.friendList.Count; i++)
			{
				if (CacheDTO.friendList[i].Name == text)
				{
					CacheDTO.friendList[i].AvatarInfo = avatarInfo;
					break;
				}
			}
			AvatarUpdatedUpdate data = new AvatarUpdatedUpdate(text, avatarInfo);
			Invoke(data);
		}
	}
}
