using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IPlatoonLeaderChangedEventListener : IServiceEventListener<string>, IServiceEventListenerBase
	{
	}
}
