using Services;
using SinglePlayerServiceLayer.EventListeners.Photon;
using SinglePlayerServiceLayer.Requests.Photon;

namespace SinglePlayerServiceLayer
{
	internal class SinglePlayerEventListenerFactory : EventListenerFactory, ISinglePlayerEventListenerFactory, IEventListenerFactory
	{
		public SinglePlayerEventListenerFactory()
		{
			Bind<ISinglePlayerSpawnRobotEventListener, SinglePlayerSpawnRobotEventListener, CommunityRobotData>();
			Bind<ISinglePlayerSpawnErrorEventListener, SinglePlayerSpawnErrorEventListener, string>();
			Bind<ISinglePlayerUpdateExperienceEventListener, SinglePlayerUpdateExperienceEventListener, int>();
			Bind<ISinglePlayerNoRobotFoundErrorEventListener, SinglePlayerNoRobotFoundErrorEventListener, string>();
		}
	}
}
