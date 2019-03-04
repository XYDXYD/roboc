using SocialServiceLayer.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IPlatoonRobotTierChangedEventListener : IServiceEventListener<PlatoonRobotTierEventData>, IServiceEventListenerBase
	{
	}
}
