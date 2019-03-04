using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanDataChangedEventListener : IServiceEventListener<ClanInfo>, IServiceEventListenerBase
	{
	}
}
