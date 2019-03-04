using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanMemberDataChangedEventListener : IServiceEventListener<ClanMember[], ClanMemberDataChangedEventContent>, IServiceEventListenerBase
	{
	}
}
