using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer
{
	internal interface ISinglePlayerNoRobotFoundErrorEventListener : IServiceEventListener<string>, IServiceEventListenerBase
	{
	}
}
