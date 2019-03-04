using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IAcknowledgeSanctionRequest : IServiceRequest<Sanction>, IServiceRequest
	{
	}
}
