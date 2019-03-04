using ChatServiceLayer.Photon;
using Services;

namespace ChatServiceLayer
{
	internal class ChatEventListenerFactory : EventListenerFactory, IChatEventListenerFactory, IEventListenerFactory
	{
		public ChatEventListenerFactory()
		{
			Bind<ISanctionEventListener, SanctionEventListener>();
			Bind<IPlayerJoinedChatRoomEventListener, PlayerJoinedChatRoomEventListener>();
			Bind<IPlayerLeftChatRoomEventListener, PlayerLeftChatRoomEventListener>();
			Bind<IPlayerChatStateEventListener, PlayerChatStateEventListener>();
		}
	}
}
