using SinglePlayerServiceLayer.Requests.Photon;
using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer
{
	internal interface ISinglePlayerSpawnRobotEventListener : IServiceEventListener<CommunityRobotData>, IServiceEventListenerBase
	{
	}
}
