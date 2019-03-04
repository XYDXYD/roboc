using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace ChatServiceLayer.Photon
{
	internal class PlayerJoinedChatRoomEventListener : ChatEventListener<PlayerJoinedChatRoomData>, IPlayerJoinedChatRoomEventListener, IServiceEventListener<PlayerJoinedChatRoomData>, IServiceEventListenerBase
	{
		public override ChatEventCode ChatEventCode => ChatEventCode.PlayerJoinedRoom;

		protected override void ParseData(EventData eventData)
		{
			string channelName = (string)eventData.Parameters[3];
			string playerName = (string)eventData.Parameters[22];
			ChatPlayerState chatPlayerState = (ChatPlayerState)eventData.Parameters[23];
			bool flag = (bool)eventData.Parameters[24];
			int avatarId = (!flag) ? ((int)eventData.Parameters[25]) : 0;
			byte[] customAvatarBytes = (!flag) ? null : ((byte[])eventData.Parameters[26]);
			Invoke(new PlayerJoinedChatRoomData(channelName, playerName, chatPlayerState, new AvatarInfo(flag, avatarId), new CustomAvatarInfo(customAvatarBytes, CustomAvatarFormat.Png)));
		}
	}
}
