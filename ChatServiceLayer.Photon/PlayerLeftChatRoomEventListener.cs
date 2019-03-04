using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace ChatServiceLayer.Photon
{
	internal class PlayerLeftChatRoomEventListener : ChatEventListener<PlayerLeftChatRoomData>, IPlayerLeftChatRoomEventListener, IServiceEventListener<PlayerLeftChatRoomData>, IServiceEventListenerBase
	{
		public override ChatEventCode ChatEventCode => ChatEventCode.PlayerLeftRoom;

		protected override void ParseData(EventData eventData)
		{
			string channelName = (string)eventData.Parameters[3];
			string playerName = (string)eventData.Parameters[22];
			Invoke(new PlayerLeftChatRoomData(channelName, playerName));
		}
	}
}
