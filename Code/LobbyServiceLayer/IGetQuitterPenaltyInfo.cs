using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IGetQuitterPenaltyInfo : IServiceRequest, IAnswerOnComplete<QuitterInfo>
	{
	}
}
