using Svelto.ServiceLayer;

namespace LobbyServiceLayer.Photon
{
	internal interface IGetReconnectableGameRequest : IServiceRequest, IAnswerOnComplete<EnterBattleDependency>
	{
	}
}
