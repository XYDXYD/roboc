using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace ChatServiceLayer.Photon
{
	internal class PlayerChatStateEventListener : ChatEventListener<PlayerChatStateUpdateData>, IPlayerChatStateEventListener, IServiceEventListener<PlayerChatStateUpdateData>, IServiceEventListenerBase
	{
		public override ChatEventCode ChatEventCode => ChatEventCode.PlayerStateUpdate;

		protected override void ParseData(EventData eventData)
		{
			string channelName = (string)eventData.Parameters[3];
			string playerName = (string)eventData.Parameters[22];
			ChatPlayerState chatPlayerState = (ChatPlayerState)eventData.Parameters[23];
			Invoke(new PlayerChatStateUpdateData(channelName, playerName, chatPlayerState));
		}
	}
}
