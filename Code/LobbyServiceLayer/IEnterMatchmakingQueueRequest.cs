using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IEnterMatchmakingQueueRequest : IServiceRequest<EnterMatchmakingQueueDependency>, IAnswerOnComplete<int>, IServiceRequest
	{
	}
}
