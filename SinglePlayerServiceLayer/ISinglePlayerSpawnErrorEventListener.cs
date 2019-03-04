using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer
{
	internal interface ISinglePlayerSpawnErrorEventListener : IServiceEventListener<string>, IServiceEventListenerBase
	{
	}
}
