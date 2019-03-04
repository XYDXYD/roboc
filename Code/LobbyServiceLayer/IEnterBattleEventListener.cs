using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IEnterBattleEventListener : IServiceEventListener<EnterBattleDependency>, IServiceEventListenerBase
	{
	}
}
