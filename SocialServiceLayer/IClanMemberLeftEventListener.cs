using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanMemberLeftEventListener : IServiceEventListener<ClanMember[], ClanMember>, IServiceEventListenerBase
	{
	}
}
