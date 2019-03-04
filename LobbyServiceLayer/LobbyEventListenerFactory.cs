using LobbyServiceLayer.Photon;
using Services;

namespace LobbyServiceLayer
{
	internal class LobbyEventListenerFactory : EventListenerFactory, ILobbyEventListenerFactory, IEventListenerFactory
	{
		public LobbyEventListenerFactory()
		{
			Bind<IBattleFoundEventListener, BattleFoundEventListener>();
			Bind<IErrorJoiningQueueEventListener, ErrorJoiningQueueEventListener>();
			Bind<IStartConnectionTestEventListener, StartConnectionTestEventListener>();
			Bind<IEnterBattleEventListener, EnterBattleEventListener>();
			Bind<IBattleCancelledEventListener, BattleCancelledEventListener>();
			Bind<IErrorInQueueEventListener, ErrorInQueueEventListener>();
			Bind<IErrorJoiningBattleEventListener, ErrorJoiningBattleEventListener>();
		}
	}
}
