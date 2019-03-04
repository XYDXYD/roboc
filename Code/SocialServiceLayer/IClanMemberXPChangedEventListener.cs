using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanMemberXPChangedEventListener : IServiceEventListener<ClanMemberXPChangedEventContent>, IServiceEventListenerBase
	{
	}
}
