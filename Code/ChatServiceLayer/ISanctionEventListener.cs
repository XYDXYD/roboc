using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface ISanctionEventListener : IServiceEventListener<Sanction>, IServiceEventListenerBase
	{
	}
}
