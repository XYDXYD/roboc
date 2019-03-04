using Svelto.ES.Legacy;

namespace Simulation
{
	internal interface IPingMessageDisplayerComponent : IComponent
	{
		void ShowPingMessage(string user, PingType type);
	}
}
