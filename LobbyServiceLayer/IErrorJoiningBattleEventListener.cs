using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IErrorJoiningBattleEventListener : IServiceEventListener<LobbyReasonCode>, IServiceEventListenerBase
	{
	}
}
