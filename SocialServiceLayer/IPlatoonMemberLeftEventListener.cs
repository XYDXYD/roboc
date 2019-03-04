using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IPlatoonMemberLeftEventListener : IServiceEventListener<string, PlatoonMember.MemberStatus>, IServiceEventListenerBase
	{
	}
}
