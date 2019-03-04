using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanMemberJoinedEventListener : IServiceEventListener<ClanMember[], ClanMember>, IServiceEventListenerBase
	{
	}
}
