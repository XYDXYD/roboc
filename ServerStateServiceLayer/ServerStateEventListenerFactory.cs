using ServerStateServiceLayer.EventListeners.Photon;
using Services;

namespace ServerStateServiceLayer
{
	internal class ServerStateEventListenerFactory : EventListenerFactory, IServerStateEventListenerFactory, IEventListenerFactory
	{
		public ServerStateEventListenerFactory()
		{
			Bind<IDevMessageEventListener, DevMessageEventListener, DevMessage>();
			Bind<IMaintenanceModeEventListener, MaintenanceModeEventListener, string>();
			Bind<IBrawlChangedEventListener, BrawlChangedEventListener, bool>();
			Bind<ICustomGameInvitationEventListener, CustomGameInvitationEventListener, CustomGameInvitationData>();
			Bind<ICustomGameRefreshEventListener, CustomGameRefreshEventListener, string>();
			Bind<ICustomGameConfigChangedEventListener, CustomGameConfigChangedEventListener, CustomGameConfigChangedData>();
			Bind<ICustomGameLeaderChangedEventListener, CustomGameLeaderChangedEventListener, string>();
			Bind<ICustomGameDeclinedInvitationEventListener, CustomGameDeclinedInvitationEventListener, DeclineInviteToSessionData>();
			Bind<ICustomGameKickedFromSessionEventListener, CustomGameKickedFromSessionEventListener, KickedFromCustomGameSessionData>();
			Bind<ICustomGameRobotTierChangedEventListener, CustomGameRobotTierChangedEventListener, CustomGameRobotTierChangedEventData>();
			Bind<ICampaignsChangedEventListener, CampaignsChangedEventListener>();
		}
	}
}
