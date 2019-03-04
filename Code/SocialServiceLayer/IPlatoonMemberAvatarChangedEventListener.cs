using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IPlatoonMemberAvatarChangedEventListener : IServiceEventListener<PlatoonMemberChangedAvatarUpdate>, IServiceEventListenerBase
	{
	}
}
