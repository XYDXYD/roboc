using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal interface ICustomGameRobotTierChangedEventListener : IServerStateEventListener<CustomGameRobotTierChangedEventData>, IServiceEventListener<CustomGameRobotTierChangedEventData>, IServiceEventListenerBase
	{
	}
}
