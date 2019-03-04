using ExitGames.Client.Photon;

namespace ChatServiceLayer
{
	internal class ChatChannelInfo
	{
		public readonly string ChannelName;

		public readonly ChatChannelType ChannelType;

		public readonly ChatChannelMember[] Members;

		public ChatChannelInfo(string channelName, ChatChannelType channelType, ChatChannelMember[] members)
		{
			ChannelName = channelName;
			ChannelType = channelType;
			Members = members;
		}

		public static ChatChannelInfo Deserialise(Hashtable channelInfo)
		{
			string channelName = (string)channelInfo.get_Item((object)"channelName");
			Hashtable[] array = (Hashtable[])channelInfo.get_Item((object)"members");
			ChatChannelType channelType = (ChatChannelType)channelInfo.get_Item((object)"channelType");
			ChatChannelMember[] array2 = new ChatChannelMember[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Hashtable val = array[i];
				string userName = (string)val.get_Item((object)"name");
				bool flag = (bool)val.get_Item((object)"useCustomAvatar");
				ChatPlayerState chatPlayerState = (ChatPlayerState)val.get_Item((object)"state");
				int avatarId = 0;
				byte[] customAvatarBytes = null;
				if (flag)
				{
					customAvatarBytes = (byte[])val.get_Item((object)"customAvatar");
				}
				else
				{
					avatarId = (int)val.get_Item((object)"avatarId");
				}
				array2[i] = new ChatChannelMember(userName, chatPlayerState, new AvatarInfo(flag, avatarId), new CustomAvatarInfo(customAvatarBytes, CustomAvatarFormat.Png));
			}
			return new ChatChannelInfo(channelName, channelType, array2);
		}
	}
}
