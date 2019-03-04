using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IGetMyAndEnemyTeamRequest : IServiceRequest, IAnswerOnComplete<int[]>
	{
	}
}
