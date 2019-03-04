using LobbyServiceLayer;
using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerEnteredGameRequest : IServiceRequest<EnterBattleDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
