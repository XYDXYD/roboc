using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IAvatarUpdatedEventListener : IServiceEventListener<AvatarUpdatedUpdate>, IServiceEventListenerBase
	{
	}
}
