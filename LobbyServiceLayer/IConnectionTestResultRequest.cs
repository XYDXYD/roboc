using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IConnectionTestResultRequest : IServiceRequest<bool>, IAnswerOnComplete, IServiceRequest
	{
	}
}
