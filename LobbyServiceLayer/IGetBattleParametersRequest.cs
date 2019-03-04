using Battle;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IGetBattleParametersRequest : IServiceRequest, IAnswerOnComplete<BattleParametersData>
	{
	}
}
