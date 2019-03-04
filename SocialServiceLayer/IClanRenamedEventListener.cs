using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanRenamedEventListener : IServiceEventListener<ClanRenameDependency>, IServiceEventListenerBase
	{
	}
}
