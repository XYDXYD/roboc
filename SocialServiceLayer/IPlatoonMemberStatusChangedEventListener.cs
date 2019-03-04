using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IPlatoonMemberStatusChangedEventListener : IServiceEventListener<string, PlatoonStatusChangedData>, IServiceEventListenerBase
	{
	}
}
